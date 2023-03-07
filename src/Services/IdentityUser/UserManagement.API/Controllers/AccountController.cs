using DataAccess.Entities;
using EventBus.Messages.Events;
using Flurl;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using UserManagement.API.DTOs.Requests;
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
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IConfiguration _configuration;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IOptions<ApiBehaviorOptions> apiBehaviorOptions,
        IEmailSender emailSender,
        IPublishEndpoint publishEndpoint,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _emailSender = emailSender;
        _apiBehaviorOptions = apiBehaviorOptions.Value;
        _publishEndpoint = publishEndpoint;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterUserRequest request)
    {
        if (!await _roleManager.RoleExistsAsync("User"))
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

        await _publishEndpoint.Publish(new UserProfileCreatedIntegrationEvent(
            Guid.Parse(user.Id),
            user.FirstName,
            user.LastName));

        return Ok("Registration successful, please check your email for verification instructions");
    }

    [HttpPost("resend-confirm-email")]
    public async Task<IActionResult> ResendConfirmEmail(ResendConfirmEmailRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
        {
            return Ok("Please check your email for verification instructions");
        }

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        await SendConfirmEmailAsync(user.Email!, $"{user.FirstName} {user.LastName}", user.Id, token);

        return Ok("Please check your email for verification instructions");
    }

    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail(ConfirmEmailRequest request)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);

        if (user == null)
        {
            return Ok("Verification successful, you can now login");
        }

        var result = await _userManager.ConfirmEmailAsync(user, request.Token);

        if (!result.Succeeded)
        {
            return BadRequest("Confirmation failed");
        }

        return Ok("Verification successful, you can now login");
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
        {
            return Ok("Please check your email for password reset instructions");
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        await SendResetPasswordEmailAsync(user.Email!, $"{user.FirstName} {user.LastName}", token);

        return Ok("Please check your email for password reset instructions");
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
        {
            return Ok("Password reset successful, you can now login");
        }

        var result = await _userManager.ResetPasswordAsync(user, request.Token, request.Password);

        if (!result.Succeeded)
        {
            AddErrors(result);
            return _apiBehaviorOptions.InvalidModelStateResponseFactory(ControllerContext);
        }

        return Ok("Password reset successful, you can now login");
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
        var url = _configuration["SpaClient"]
            .AppendPathSegment("confirm-email")
            .SetQueryParams(new
            {
                userId,
                token
            });

        var message = new Message(
            new EmailAddress[]
            {
                new EmailAddress
                {
                    DisplayName = displayReceiverName,
                    Address = email
                }
            },
            "Confirm your email",
            $"Please confirm your account by <a href='{url}'>clicking here</a>.");

        await _emailSender.SendEmailAsync(message);
    }

    private async Task SendResetPasswordEmailAsync(string email, string displayReceiverName, string token)
    {
        var url = _configuration["SpaClient"]
            .AppendPathSegment("reset-password")
            .SetQueryParams(new
            {
                email,
                token
            });

        var message = new Message(
            new EmailAddress[]
            {
                new EmailAddress
                {
                    DisplayName = displayReceiverName,
                    Address = email
                }
            },
            "Reset Password",
            $"Please reset your password by <a href='{url}'>clicking here</a>.");

        await _emailSender.SendEmailAsync(message);
    }

    #endregion
}
