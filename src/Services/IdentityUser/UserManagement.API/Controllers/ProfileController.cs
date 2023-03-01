using DataAccess.Entities;
using EventBus.Messages.Events;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using UserManagement.API.DTOs.Requests;
using UserManagement.API.DTOs.Responses;
using UserManagement.API.Infrastructure.Attributes;
using UserManagement.API.Infrastructure.Services;

namespace UserManagement.API.Controllers;

[Authorize]
[Route("api/v1/[controller]")]
[ApiController]
public class ProfileController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApiBehaviorOptions _apiBehaviorOptions;
    private readonly IIdentityService _identityService;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public ProfileController(
        UserManager<ApplicationUser> userManager,
        IOptions<ApiBehaviorOptions> apiBehaviorOptions,
        IIdentityService identityService,
        IPublishEndpoint publishEndpoint,
        IWebHostEnvironment webHostEnvironment)
    {
        _userManager = userManager;
        _apiBehaviorOptions = apiBehaviorOptions.Value;
        _identityService = identityService;
        _publishEndpoint = publishEndpoint;
        _webHostEnvironment = webHostEnvironment;
    }

    [HttpGet("full-name")]
    public async Task<ActionResult<ProfileFullnameResponse>> GetProfileFullname()
    {
        var userid = _identityService.GetUserIdentity();
        var user = await _userManager.FindByIdAsync(userid);

        if (user == null)
        {
            return NotFound("No user found");
        }

        return new ProfileFullnameResponse
        {
            LastName = user.LastName,
            FirstName = user.FirstName,
        };
    }

    [HttpPut("full-name")]
    public async Task<IActionResult> PutProfileFullname(UpdateProfileFullnameRequest request)
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

    [HttpPut("password")]
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

    [HttpPut("photo")]
    public async Task<IActionResult> PutPhoto(
        [MaxFileSize(2 * 1024 * 1024)]
        [AllowedExtensions(".jpg", ".jpeg", ".png")]
        IFormFile file)
    {
        var userid = _identityService.GetUserIdentity();
        var user = await _userManager.FindByIdAsync(userid);

        if (user == null)
        {
            return NotFound("No user found");
        }

        var photoDicrectoryPath = Path.Combine(_webHostEnvironment.WebRootPath, "Pictures", "Photos", userid);

        if (!Directory.Exists(photoDicrectoryPath))
        {
            Directory.CreateDirectory(photoDicrectoryPath);
        }

        if (user.PhotoFileName != null)
        {
            var filePath = Path.Combine(photoDicrectoryPath, user.PhotoFileName);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }

        var newFilePath = Path.Combine(photoDicrectoryPath, file.FileName);

        using var stream = System.IO.File.Create(newFilePath);
        await file.CopyToAsync(stream);

        user.PhotoFileName = file.FileName;
        await _userManager.UpdateAsync(user);

        return NoContent();
    }

    [HttpDelete("photo")]
    public async Task<IActionResult> DeletePhoto()
    {
        var userid = _identityService.GetUserIdentity();
        var user = await _userManager.FindByIdAsync(userid);

        if (user == null)
        {
            return NotFound("No user found");
        }

        var photoDicrectoryPath = Path.Combine(_webHostEnvironment.WebRootPath, "Pictures", "Photos", userid);

        if (!Directory.Exists(photoDicrectoryPath))
        {
            Directory.CreateDirectory(photoDicrectoryPath);
        }

        if (user.PhotoFileName != null)
        {
            var filePath = Path.Combine(photoDicrectoryPath, user.PhotoFileName);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            user.PhotoFileName = null;
            await _userManager.UpdateAsync(user);
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

    #endregion
}
