using Testing.Core.Bases;

namespace Testing.Core.Domain.AggregatesModel.SolutionAggregate;

public class Solution : Entity, IAggregateRoot
{
    public User User { get; private set; }

    public Guid TaskId => _taskId;
    private Guid _taskId;

    public SolutionValue Value { get; private set; }
    public bool Success { get; private set; }

    public Solution(
        Guid id,
        Guid taskId,
        User user,
        SolutionValue value,
        bool success)
    {
        Value = value;
        Id = id;
        Success = success;

        User = user;
        _taskId = taskId;
    }
}
