using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using UserManagement.API.Infrastructure.ActionResults;

namespace UserManagement.API.Infrastructure.Filters;

public class HttpGlobalExceptionFilter : IExceptionFilter
{
    private readonly IWebHostEnvironment env;
    private readonly ILogger<HttpGlobalExceptionFilter> logger;

    public HttpGlobalExceptionFilter(IWebHostEnvironment env, ILogger<HttpGlobalExceptionFilter> logger)
    {
        this.env = env;
        this.logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        logger.LogError(new EventId(context.Exception.HResult),
            context.Exception,
            context.Exception.Message);

        var json = new JsonErrorResponse
        {
            Messages = new[] { "An error ocurred." }
        };

        if (env.IsDevelopment())
        {
            json.DeveloperMessage = context.Exception.Message;
        }

        context.Result = new InternalServerErrorObjectResult(json);
        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        context.ExceptionHandled = true;
    }

    private class JsonErrorResponse
    {
        public string[] Messages { get; set; } = Array.Empty<string>();

        public string? DeveloperMessage { get; set; }
    }
}