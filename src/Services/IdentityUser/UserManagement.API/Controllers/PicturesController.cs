using DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace UserManagement.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class PicturesController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public PicturesController(
        UserManager<ApplicationUser> userManager,
        IWebHostEnvironment webHostEnvironment)
    {
        _userManager = userManager;
        _webHostEnvironment = webHostEnvironment;
    }

    [HttpGet]
    [Route("{userId:guid}/photo")]
    public async Task<ActionResult> GetPhoto(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null || user.PhotoFileName == null)
        {
            return NotFound();
        }

        var path = Path.Combine(_webHostEnvironment.WebRootPath, "Pictures", "Photos", user.Id, user.PhotoFileName);

        string imageFileExtension = Path.GetExtension(user.PhotoFileName);
        string mimetype = GetImageMimeTypeFromImageFileExtension(imageFileExtension);

        var buffer = await System.IO.File.ReadAllBytesAsync(path);

        return File(buffer, mimetype);
    }

    private string GetImageMimeTypeFromImageFileExtension(string extension)
    {
        string mimetype = extension switch
        {
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".bmp" => "image/bmp",
            ".tiff" => "image/tiff",
            ".wmf" => "image/wmf",
            ".jp2" => "image/jp2",
            ".svg" => "image/svg+xml",
            _ => "application/octet-stream",
        };
        return mimetype;
    }
}
