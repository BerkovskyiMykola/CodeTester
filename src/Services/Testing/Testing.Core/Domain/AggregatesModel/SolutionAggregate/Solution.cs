﻿using Common.Models.Domain;

namespace Testing.Core.Domain.AggregatesModel.SolutionAggregate;

public class Solution : Entity, IAggregateRoot
{
    public Guid UserId => _userId;
    private Guid _userId;

    public Guid TaskId => _taskId;
    private Guid _taskId;

    public SolutionValue Value { get; private set; }
    public bool Success { get; private set; }

    public DateTime CreateDate { get; private set; }

    //Only for migrations
    #pragma warning disable CS8618
    protected Solution() { }
    #pragma warning restore CS8618

    public Solution(
        Guid id,
        Guid taskId,
        Guid userId,
        SolutionValue value,
        bool success)
    {
        Id = id;
        Value = value;
        Success = success;
        _userId = userId;
        _taskId = taskId;

        CreateDate = DateTime.UtcNow;
    }
}
