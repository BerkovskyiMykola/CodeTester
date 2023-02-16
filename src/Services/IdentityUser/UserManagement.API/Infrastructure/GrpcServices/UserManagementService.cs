using DataAccess.Data;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using UserManagement.API.Protos;

namespace UserManagement.API.Infrastructure.GrpcServices;

public class UserManagementService : UserManagementGrpc.UserManagementGrpcBase
{
    private readonly ApplicationContext _context;

    public UserManagementService(ApplicationContext context)
    {
        _context = context;
    }

    public override async Task<UserResponse> GetUserById(UserIdRequest request, ServerCallContext context)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == request.Id);

        if (user == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"User with Id={request.Id} is not found."));
        }

        return new UserResponse
        {
            Id = user.Id,
            Email = user.Email,
            Lastname = user.LastName,
            Firstname = user.FirstName,
        };
    }
}
