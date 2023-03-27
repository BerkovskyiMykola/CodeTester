using DataAccess.Entities;
using Duende.IdentityServer;
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Services;
using EventBus.Messages.Events;
using IdentityModel;
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace Identity.API.Pages.ExternalLogin;

#nullable disable

[AllowAnonymous]
[SecurityHeaders]
public class Callback : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IIdentityServerInteractionService _interaction;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<Callback> _logger;
    private readonly IEventService _events;

    public Callback(
        IIdentityServerInteractionService interaction,
        IEventService events,
        ILogger<Callback> logger,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IPublishEndpoint publishEndpoint)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _interaction = interaction;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
        _events = events;
    }

    public async Task<IActionResult> OnGet()
    {
        // read external identity from the temporary cookie
        var result = await HttpContext.AuthenticateAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);
        if (result?.Succeeded != true)
        {
            throw new Exception("External authentication error");
        }

        var externalUser = result.Principal;

        if (_logger.IsEnabled(LogLevel.Debug))
        {
            var externalClaims = externalUser.Claims.Select(c => $"{c.Type}: {c.Value}");
            _logger.LogDebug("External claims: {@claims}", externalClaims);
        }

        // lookup our user and external provider info
        // try to determine the unique id of the external user (issued by the provider)
        // the most common claim type for that are the sub claim and the NameIdentifier
        // depending on the external provider, some other claim type might be used
        var userIdClaim = externalUser.FindFirst(JwtClaimTypes.Subject) ??
                          externalUser.FindFirst(ClaimTypes.NameIdentifier) ??
                          throw new Exception("Unknown userid");

        var provider = result.Properties.Items["scheme"];
        var providerUserId = userIdClaim.Value;

        // find external user
        var user = await _userManager.FindByLoginAsync(provider, providerUserId);
        if (user == null)
        {
            // this might be where you might initiate a custom workflow for user registration
            // in this sample we don't show how that would be done, as our sample implementation
            // simply auto-provisions new external user
            user = await AutoProvisionUserAsync(provider, providerUserId, externalUser.Claims);
        }

        // this allows us to collect any additional claims or properties
        // for the specific protocols used and store them in the local auth cookie.
        // this is typically used to store data needed for signout from those protocols.
        var additionalLocalClaims = new List<Claim>();
        var localSignInProps = new AuthenticationProperties();
        CaptureExternalLoginContext(result, additionalLocalClaims, localSignInProps);

        // issue authentication cookie for user
        await _signInManager.SignInWithClaimsAsync(user, localSignInProps, additionalLocalClaims);

        // delete temporary cookie used during external authentication
        await HttpContext.SignOutAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);

        // retrieve return URL
        var returnUrl = result.Properties.Items["returnUrl"] ?? "~/";

        // check if external login is in the context of an OIDC request
        var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
        await _events.RaiseAsync(new UserLoginSuccessEvent(provider, providerUserId, user.Id, user.UserName, true, context?.Client.ClientId));

        if (context != null)
        {
            if (context.IsNativeClient())
            {
                // The client is native, so this change in how to
                // return the response is for better UX for the end user.
                return this.LoadingPage(returnUrl);
            }
        }

        return Redirect(returnUrl);
    }

    private async Task<ApplicationUser> AutoProvisionUserAsync(string provider, string providerUserId, IEnumerable<Claim> claims)
    {
        var sub = Guid.NewGuid().ToString();
        var user = new ApplicationUser
        {
            Id = sub,
            UserName = sub
        };

        var email = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Email)?.Value ??
                    claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;

        if (email != null)
        {
            user.Email = email;
            user.EmailConfirmed = true;
        }

        var firstname = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.GivenName)?.Value ??
                        claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName)?.Value;
        var lastname = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.FamilyName)?.Value ??
                   claims.FirstOrDefault(x => x.Type == ClaimTypes.Surname)?.Value;

        user.Fullname = $"{firstname} {lastname}";

        var identityResult = await _userManager.CreateAsync(user);
        if (!identityResult.Succeeded) throw new Exception(identityResult.Errors.First().Description);

        identityResult = await _userManager.AddToRoleAsync(user, "User");
        if (!identityResult.Succeeded) throw new Exception(identityResult.Errors.First().Description);

        identityResult = await _userManager.AddLoginAsync(user, new UserLoginInfo(provider, providerUserId, provider));
        if (!identityResult.Succeeded) throw new Exception(identityResult.Errors.First().Description);

        await _publishEndpoint.Publish(new UserProfileCreatedIntegrationEvent(
            Guid.Parse(user.Id),
            user.Fullname));

        return user;
    }

    // if the external login is OIDC-based, there are certain things we need to preserve to make logout work
    // this will be different for WS-Fed, SAML2p or other protocols
    private void CaptureExternalLoginContext(AuthenticateResult externalResult, List<Claim> localClaims, AuthenticationProperties localSignInProps)
    {
        // capture the idp used to login, so the session knows where the user came from
        localClaims.Add(new Claim(JwtClaimTypes.IdentityProvider, externalResult.Properties.Items["scheme"]));

        // if the external system sent a session id claim, copy it over
        // so we can use it for single sign-out
        var sid = externalResult.Principal.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.SessionId);
        if (sid != null)
        {
            localClaims.Add(new Claim(JwtClaimTypes.SessionId, sid.Value));
        }

        // if the external provider issued an id_token, we'll keep it for signout
        var idToken = externalResult.Properties.GetTokenValue("id_token");
        if (idToken != null)
        {
            localSignInProps.StoreTokens(new[] { new AuthenticationToken { Name = "id_token", Value = idToken } });
        }
    }
}