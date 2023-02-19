using Testing.API.DTOs.Tasks;

namespace Testing.API.Application.Queries.Tasks;

public interface ITaskQueries
{
    Task<TaskResponse> GetTaskAsync(Guid id);

    Task<IEnumerable<TaskResponse>> GetAllTasksAsync();
}
