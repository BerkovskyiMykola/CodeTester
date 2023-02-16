﻿namespace Dictionary.API.Infrastructure.Entities;

public class ProgrammingLanguage
{
    public int Id { get; private set; }
    public string Name { get; private set; }

    public ProgrammingLanguage(string name)
    {
        ValidateName(name);

        Name = name;
    }

    public void SetNewName(string name)
    {
        ValidateName(name);
    }

    private void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Length > 50)
            throw new InvalidOperationException("The name is invalid");
    }
}
