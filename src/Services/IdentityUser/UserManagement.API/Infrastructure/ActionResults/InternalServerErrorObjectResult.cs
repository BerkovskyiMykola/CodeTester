using Microsoft.AspNetCore.Mvc;

namespace UserManagement.API.Infrastructure.ActionResults;

public class InternalServerErrorObjectResult : ObjectResult
{
    public InternalServerErrorObjectResult(object error)
        : base(error)
    {
        StatusCode = StatusCodes.Status500InternalServerError;
    }
}
