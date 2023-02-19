using Testing.API.DTOs.Solutions;

namespace Testing.API.Application.Queries.Solutions;

public interface ISolutionQueries
{
    Task<SolutionResponse> GetSolutionAsync(Guid id);

    Task<IEnumerable<SolutionResponse>> GetAllSolutionsAsync();
}
