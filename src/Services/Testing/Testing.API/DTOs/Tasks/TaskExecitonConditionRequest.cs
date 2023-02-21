using System.ComponentModel.DataAnnotations;

namespace Testing.API.DTOs.Tasks;

public class TaskExecitonConditionRequest
{
    [Required]
    public string Tests { get; set; } = string.Empty;
    [Required]
    [Range(typeof(TimeSpan), "00:00:01", "23:59")]
    public TimeSpan TimeLimit { get; set; }
}