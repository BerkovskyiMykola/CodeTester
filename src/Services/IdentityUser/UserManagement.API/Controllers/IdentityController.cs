using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UserManagement.API.Controllers;

[Route("api/v1/[controller]")]
[Authorize]
public class IdentityController : ControllerBase
{
    [HttpGet("Claims")]
    public IActionResult Get()
    {
        return new JsonResult(from c in User.Claims select new { c.Type, c.Value });
    }
}
