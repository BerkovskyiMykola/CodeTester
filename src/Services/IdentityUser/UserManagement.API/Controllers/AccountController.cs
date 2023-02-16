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
    private readonly ApiBehaviorOptions _apiBehaviorOptions;
    private readonly IEmailSender _emailSender;
    private readonly IIdentityService _identityService;
    private readonly IPublishEndpoint _publishEndpoint;
    public AccountController(
        UserManager<ApplicationUser> userManager,
        IOptions<ApiBehaviorOptions> apiBehaviorOptions,
        IEmailSender emailSender,
        IIdentityService identityService,
        IPublishEndpoint publishEndpoint)
    {
        _userManager = userManager;
        _emailSender = emailSender;
        _apiBehaviorOptions = apiBehaviorOptions.Value;
        _identityService = identityService;
        _publishEndpoint = publishEndpoint;
    }

    [HttpPost("register-user")]
    public async Task<IActionResult> RegisterUser(RegisterUserRequest request)
    {
        var user = new ApplicationUser
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            UserName = request.Email,
            Email = request.Email,
            EmailConfirmed = false
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            AddErrors(result);
            return _apiBehaviorOptions.InvalidModelStateResponseFactory(ControllerContext);
        }

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        var confirmationLink = $"Email:{user.Email} Token:{token}";

        var message = new Message(
            new EmailAddress[]
            {
                new EmailAddress
                {
                    DisplayName = $"{user.FirstName} {user.LastName}",
                    Address = user.Email
                }
            },
            "Confirmation email link",
            confirmationLink!);

        await _userManager.AddToRoleAsync(user, "User");

        await _emailSender.SendEmailAsync(message);

        return NoContent();
    }

    [HttpPost("resend-confirm-email")]
    public async Task<IActionResult> ResendConfirmEmail(ResendConfirmEmailRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
        {
            return NotFound("User was not found.");
        }

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        var confirmationLink = $"Email:{user.Email} Token:{token}";

        var message = new Message(
            new EmailAddress[]
            {
                new EmailAddress
                {
                    DisplayName = $"{user.FirstName} {user.LastName}",
                    Address = user.Email!
                }
            },
            "Confirmation email link",
            confirmationLink);

        await _emailSender.SendEmailAsync(message);

        return NoContent();
    }

    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail(ConfirmEmailRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
        {
            return NotFound("User was not found.");
        }

        var result = await _userManager.ConfirmEmailAsync(user, request.Token);

        if (!result.Succeeded)
        {
            return BadRequest("Confirmation failed.");
        }

        await _publishEndpoint.Publish(new UserCreatedIntegrationEvent(
            Guid.Parse(user.Id),
            user.Email!,
            user.FirstName,
            user.LastName));

        return NoContent();
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
        {
            return NotFound("User was not found.");
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        var callback = $"Email:{user.Email} Token:{token}";

        var message = new Message(
            new EmailAddress[]
            {
                new EmailAddress
                {
                    DisplayName = $"{user.FirstName} {user.LastName}",
                    Address = user.Email!
                }
            },
            "Reset password token",
            callback);

        await _emailSender.SendEmailAsync(message);

        return NoContent();
    }

    [HttpPost("confirm-reset-password")]
    public async Task<IActionResult> ConfirmResetPassword(ResetPasswordConfirmRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
        {
            return NotFound("User was not found.");
        }

        var result = await _userManager.ResetPasswordAsync(user, request.Token, request.Password);

        if (!result.Succeeded)
        {
            AddErrors(result);
            return _apiBehaviorOptions.InvalidModelStateResponseFactory(ControllerContext);
        }

        return NoContent();
    }

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
            return NotFound();
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
            return NotFound();
        }

        var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.TryAddModelError(error.Code, error.Description);
            }
            return _apiBehaviorOptions.InvalidModelStateResponseFactory(ControllerContext);
        }

        return NoContent();
    }

    private void AddErrors(IdentityResult result)
    {
        foreach (var error in result.Errors)
        {
            ModelState.TryAddModelError(error.Code, error.Description);
        }
    }
}
