using DataAccess.Data;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using UserManagement.API.Protos;

namespace UserManagement.API.Infrastructure.GrpcServices;

public class UserManagementService : UserManagementGrpc.UserManagementGrpcBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ApplicationDbContext> _logger;

    public UserManagementService(
        ApplicationDbContext context,
        ILogger<ApplicationDbContext> logger)
    {
        _context = context;
        _logger = logger;
    }

    public override async Task<UserResponse> GetUserById(UserIdRequest request, ServerCallContext context)
    {
        _logger.LogInformation($"Begin grpc call UserManagementService.GetUserById for difficulty id {request.Id}.");

        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == request.Id);

        if (user == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"User with Id={request.Id} is not found."));
        }

        _logger.LogInformation($"User with Id={user.Id} is found.");

        return new UserResponse
        {
            Id = user.Id,
            Email = user.Email,
            Lastname = user.LastName,
            Firstname = user.FirstName,
        };
    }
}
