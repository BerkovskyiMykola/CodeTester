using Testing.API.DTOs.DictionaryData;

namespace Testing.API.Services;

public interface IDictionaryService
{
    Task<DifficultyData?> GetDifficultyByIdAsync(int id);

    Task<ProgrammingLanguageData?> GetProgrammingLanguageByIdAsync(int id);

    Task<TaskTypeData?> GetTaskTypeByIdAsync(int id);
}
