﻿using Core.Bases;

namespace Core.Domain.AggregatesModel.TaskAggregate;

public class Task : Entity, IAggregateRoot
{
    public Title Title { get; private set; }
    public Description Description { get; private set; }
    public Difficulty Difficulty { get; private set; }
    public Type Type { get; private set; }
    public ProgrammingLanguage ProgrammingLanguage { get; private set; }
    public SolutionExample SolutionExample { get; private set; }
    public ExecutionCondition ExecutionCondition { get; private set; }

    public DateTime CreateDate { get; private set; }

    public Task(
        Guid id,
        Title title,
        Description description,
        Difficulty difficulty,
        Type type,
        ProgrammingLanguage programmingLanguage,
        SolutionExample solutionExample,
        ExecutionCondition executionCondition)
    {
        Id = id;
        Title = title;
        Description = description;
        Difficulty = difficulty;
        Type = type;
        ProgrammingLanguage = programmingLanguage;
        SolutionExample = solutionExample;
        ExecutionCondition = executionCondition;

        CreateDate = DateTime.UtcNow;
    }

    public void SetNewTitle(Title title)
    {
        Title = title;
    }

    public void SetNewDescription(Description description)
    {
        Description = description;
    }

    public void SetNewDifficulty(Difficulty difficulty)
    {
        Difficulty = difficulty;
    }

    public void SetNewType(Type type)
    {
        Type = type;
    }

    public void SetNewProgrammingLanguage(ProgrammingLanguage programmingLanguage)
    {
        ProgrammingLanguage = programmingLanguage;
    }

    public void SetNewSolutionExample(SolutionExample solutionExample)
    {
        SolutionExample = solutionExample;
    }

    public void SetNewExecutionCondition(ExecutionCondition executionCondition)
    {
        ExecutionCondition = executionCondition;
    }
}
