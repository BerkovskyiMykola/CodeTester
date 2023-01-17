using DataAccess.Entities;
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
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailSender _emailSender;
    private readonly ApiBehaviorOptions _apiBehaviorOptions;
    private readonly IIdentityService _identityService;

    public ProfileController(
        UserManager<ApplicationUser> userManager,
        IEmailSender emailSender,
        IOptions<ApiBehaviorOptions> apiBehaviorOptions,
        IIdentityService identityService)
    {
        _userManager = userManager;
        _emailSender = emailSender;
        _apiBehaviorOptions = apiBehaviorOptions.Value;
        _identityService = identityService;
    }

    [HttpGet]
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

    [HttpPut]
    public async Task<IActionResult> PutProfile(UpdateProfileRequest request)
    {
        var userid = _identityService.GetUserIdentity();
        var user = await _userManager.FindByIdAsync(userid);

        if (user == null)
        {
            return NotFound();
        }

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;

        var result = await _userManager.UpdateAsync(user);

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

    [HttpGet("Claims")]
    public IActionResult Get()
    {
        return new JsonResult(from c in User.Claims select new { c.Type, c.Value });
    }
}
