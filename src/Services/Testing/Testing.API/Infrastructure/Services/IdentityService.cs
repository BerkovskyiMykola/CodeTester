namespace Testing.API.Infrastructure.Services;

public interface IIdentityService
{
    string GetUserIdentity();
}

public class IdentityService : IIdentityService
{
    private IHttpContextAccessor _context;

    public IdentityService(IHttpContextAccessor context)
    {
        _context = context;
    }

    public string GetUserIdentity()
    {
        return _context.HttpContext!.User.FindFirst("sub")!.Value;
    }
}
