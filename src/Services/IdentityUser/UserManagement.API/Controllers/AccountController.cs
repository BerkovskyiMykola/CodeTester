using DataAccess.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using UserManagement.API.DTO.Requests;
using UserManagement.API.DTO.Responses;
using UserManagement.API.EmailService;
using UserManagement.API.IdentityService;

namespace UserManagement.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailSender _emailSender;
    private readonly ApiBehaviorOptions _apiBehaviorOptions;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        IEmailSender emailSender,
        IOptions<ApiBehaviorOptions> apiBehaviorOptions,
        IIdentityService identityService)
    {
        _userManager = userManager;
        _emailSender = emailSender;
        _apiBehaviorOptions = apiBehaviorOptions.Value;
    }

    [HttpPost("register-user")]
    public async Task<IActionResult> RegisterUser(RegisterUserRequest model)
    {
        var user = new ApplicationUser
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            UserName = model.Email,
            Email = model.Email,
            EmailConfirmed = false
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.TryAddModelError(error.Code, error.Description);
            }
            return _apiBehaviorOptions.InvalidModelStateResponseFactory(ControllerContext);
        }

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        //var confirmationLink = Url.Action(
        //    action: nameof(ConfirmEmail), 
        //    controller: "Account", 
        //    values: new { token, email = user.Email }, 
        //    protocol: "https", 
        //    host: "our-website");

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
    public async Task<IActionResult> ResendConfirmEmail(ResendConfirmEmailRequest model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);

        if (user == null)
        {
            return NotFound("User was not found.");
        }

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        //var confirmationLink = Url.Action(
        //    action: nameof(ConfirmEmail), 
        //    controller: "Account", 
        //    values: new { token, email = user.Email }, 
        //    protocol: "https", 
        //    host: "our-website");

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
            confirmationLink!);

        await _emailSender.SendEmailAsync(message);

        return NoContent();
    }

    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail(ConfirmEmailRequest model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);

        if (user == null)
        {
            return NotFound("User was not found.");
        }

        var result = await _userManager.ConfirmEmailAsync(user, model.Token);

        if (!result.Succeeded)
        {
            return BadRequest("Confirmation failed.");
        }

        return NoContent();
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);

        if (user == null)
        {
            return NotFound("User was not found.");
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        //var callback = Url.Action(
        //    action: nameof(ResetPassword),
        //    controller: "Account",
        //    values: new { token, email = user.Email },
        //    protocol: "https",
        //    host: "our-website");

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
            callback!);

        await _emailSender.SendEmailAsync(message);

        return NoContent();
    }

    [HttpPost("reset-password-confirm")]
    public async Task<IActionResult> ResetPasswordConfirm(ResetPasswordConfirmRequest model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);

        if (user == null)
        {
            return NotFound("User was not found.");
        }

        var resetPassResult = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);

        if (!resetPassResult.Succeeded)
        {
            foreach (var error in resetPassResult.Errors)
            {
                ModelState.TryAddModelError(error.Code, error.Description);
            }

            return _apiBehaviorOptions.InvalidModelStateResponseFactory(ControllerContext);
        }

        return NoContent();
    }
}
