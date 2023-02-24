using DataAccess.Entities;
using EventBus.Messages.Events;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using UserManagement.API.DTOs.Requests;
using UserManagement.API.DTOs.Responses;
using UserManagement.API.Infrastructure.Services;
using UserManagement.API.Infrastructure.Services.EmailService;

namespace UserManagement.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApiBehaviorOptions _apiBehaviorOptions;
    private readonly IEmailSender _emailSender;
    private readonly IIdentityService _identityService;
    private readonly IPublishEndpoint _publishEndpoint;
    public AccountController(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IOptions<ApiBehaviorOptions> apiBehaviorOptions,
        IEmailSender emailSender,
        IIdentityService identityService,
        IPublishEndpoint publishEndpoint)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _emailSender = emailSender;
        _apiBehaviorOptions = apiBehaviorOptions.Value;
        _identityService = identityService;
        _publishEndpoint = publishEndpoint;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterUserRequest request)
    {
        if(!await _roleManager.RoleExistsAsync("User"))
        {
            return BadRequest("The specified role does not exist");
        }

        var user = new ApplicationUser
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            UserName = request.Email,
            Email = request.Email
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            AddErrors(result);
            return _apiBehaviorOptions.InvalidModelStateResponseFactory(ControllerContext);
        }

        result = await _userManager.AddToRoleAsync(user, "User");
        if (!result.Succeeded)
        {
            AddErrors(result);
            return _apiBehaviorOptions.InvalidModelStateResponseFactory(ControllerContext);
        }

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        await SendConfirmEmailAsync(user.Email, $"{user.FirstName} {user.LastName}", user.Id, token);

        await _publishEndpoint.Publish(new UserCreatedIntegrationEvent(
            Guid.Parse(user.Id),
            user.Email!,
            user.FirstName,
            user.LastName));

        return NoContent();
    }

    [HttpPost("resend-confirm-email")]
    public async Task<IActionResult> ResendConfirmEmail(ResendConfirmEmailRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
        {
            return NotFound("No user found");
        }

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        await SendConfirmEmailAsync(user.Email!, $"{user.FirstName} {user.LastName}", user.Id, token);

        return NoContent();
    }

    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail(ConfirmEmailRequest request)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);

        if (user == null)
        {
            return NotFound("No user found");
        }

        var result = await _userManager.ConfirmEmailAsync(user, request.Token);

        if (!result.Succeeded)
        {
            return BadRequest("Confirmation failed");
        }

        return NoContent();
    }

    // TODO
    // Should be in Identity.API maybe

    //[HttpPost("forgot-password")]
    //public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
    //{
    //    var user = await _userManager.FindByEmailAsync(request.Email);

    //    if (user == null)
    //    {
    //        return NotFound("No user found");
    //    }

    //    if (user.Email == null)
    //    {
    //        return BadRequest("The user does not have an email address");
    //    }

    //    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
    //    await SendResetPasswordEmailAsync(user.Email, $"{user.FirstName} {user.LastName}", token);

    //    return NoContent();
    //}

    //[HttpPost("confirm-reset-password")]
    //public async Task<IActionResult> ConfirmResetPassword(ResetPasswordConfirmRequest request)
    //{
    //    var user = await _userManager.FindByEmailAsync(request.Email);

    //    if (user == null)
    //    {
    //        return NotFound("No user found");
    //    }

    //    var result = await _userManager.ResetPasswordAsync(user, request.Token, request.Password);

    //    if (!result.Succeeded)
    //    {
    //        AddErrors(result);
    //        return _apiBehaviorOptions.InvalidModelStateResponseFactory(ControllerContext);
    //    }

    //    return NoContent();
    //}

    [HttpGet("profile")]
    [Authorize]
    public async Task<ActionResult<ProfileResponse>> GetProfile()
    {
        var userid = _identityService.GetUserIdentity();
        var user = await _userManager.FindByIdAsync(userid);

        if (user == null)
        {
            return NotFound();
        }

        return new ProfileResponse
        {
            LastName = user.LastName,
            FirstName = user.FirstName,
        };
    }

    [HttpPut("profile")]
    [Authorize]
    public async Task<IActionResult> PutProfile(UpdateProfileRequest request)
    {
        var userId = _identityService.GetUserIdentity();
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return NotFound("No user found");
        }

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            AddErrors(result);
            return _apiBehaviorOptions.InvalidModelStateResponseFactory(ControllerContext);
        }

        await _publishEndpoint.Publish(new UserProfileUpdatedIntegrationEvent(
            Guid.Parse(userId),
            user.FirstName,
            user.LastName));

        return NoContent();
    }

    [HttpPut("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
    {
        var userid = _identityService.GetUserIdentity();
        var user = await _userManager.FindByIdAsync(userid);

        if (user == null)
        {
            return NotFound("No user found");
        }

        var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

        if (!result.Succeeded)
        {
            AddErrors(result);
            return _apiBehaviorOptions.InvalidModelStateResponseFactory(ControllerContext);
        }

        return NoContent();
    }

    #region Help methods

    private void AddErrors(IdentityResult result)
    {
        foreach (var error in result.Errors)
        {
            ModelState.TryAddModelError(error.Code, error.Description);
        }
    }

    private async Task SendConfirmEmailAsync(string email, string displayReceiverName, string userId, string token)
    {
        var confirmationLink = $"UserId:{userId} Token:{token}";

        var message = new Message(
            new EmailAddress[]
            {
                new EmailAddress
                {
                    DisplayName = displayReceiverName,
                    Address = email
                }
            },
            "Confirm email",
            confirmationLink!);

        await _emailSender.SendEmailAsync(message);
    }

    private async Task SendResetPasswordEmailAsync(string email, string displayReceiverName, string token)
    {
        var callback = $"Email:{email} Token:{token}";

        var message = new Message(
            new EmailAddress[]
            {
                new EmailAddress
                {
                    DisplayName = displayReceiverName,
                    Address = email
                }
            },
            "Reset password",
            callback);

        await _emailSender.SendEmailAsync(message);
    }

    #endregion
}
