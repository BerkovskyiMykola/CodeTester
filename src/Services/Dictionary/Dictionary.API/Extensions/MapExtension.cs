using Dictionary.API.DTO.Responses;
using Dictionary.API.Entities;

namespace Dictionary.API.Extensions
{
    public static class MapExtension
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
