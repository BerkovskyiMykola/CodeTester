using Dictionary.API.DTOs.Difficulties;
using Dictionary.API.DTOs.ProgrammingLanguages;
using Dictionary.API.DTOs.TaskTypes;
using Dictionary.API.Infrastructure.Entities;

namespace Dictionary.API.Extensions
{
    public static class MappingExtensions
    {
        public static IQueryable<DifficultyResponse> MapToDifficultyResponse(this IQueryable<Difficulty> difficulties)
        {
            return difficulties.Select(x => new DifficultyResponse { Id = x.Id, Name = x.Name });
        }

        public static IQueryable<ProgrammingLanguageResponse> MapToProgrammingLanguageResponse(this IQueryable<ProgrammingLanguage> programmingLanguages)
        {
            return programmingLanguages.Select(x => new ProgrammingLanguageResponse { Id = x.Id, Name = x.Name });
        }

        public static IQueryable<TaskTypeResponse> MapToTaskTypeResponse(this IQueryable<TaskType> taskTypes)
        {
            return taskTypes.Select(x => new TaskTypeResponse { Id = x.Id, Name = x.Name });
        }
    }
}
