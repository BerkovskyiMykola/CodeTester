﻿using Core.Bases;
using Core.Domain.AggregatesModel.TaskAggregate;

namespace Core.Domain.AggregatesModel.SolutionAggregate;

public record User
{
    public Guid Id { get; init; }
    public string Email { get; init; }
    public string Lastname { get; init; }
    public string Firstname { get; init; }

    private User(Guid id, string email, string lastname, string firstname)
        => (Id, Email, Lastname, Firstname) = (id, email, lastname, firstname);

    public static Result<User> Create(Guid id, string email, string lastname, string firstname)
    {
        if (string.IsNullOrWhiteSpace(email))
            return Result.Fail<User>("Email can't be empty");

        if (string.IsNullOrWhiteSpace(email))
            return Result.Fail<User>("Email is invalid");

        if (string.IsNullOrWhiteSpace(lastname))
            return Result.Fail<User>("Lastname is invalid");

        if (lastname.Length > 50)
            return Result.Fail<User>("Lastname is too long");

        if (string.IsNullOrWhiteSpace(firstname))
            return Result.Fail<User>("Firstname is invalid");

        if (firstname.Length > 50)
            return Result.Fail<User>("Firstname is too long");


        return Result.Ok(new User(id, email, lastname, firstname));
    }

    public bool IsValidEmail(string value)
    {
        if (value.Length == 0)
        {
            return false;
        }

        int index = value.IndexOf('@');

        return
            index > 0 &&
            index != value.Length - 1 &&
            index == value.LastIndexOf('@');
    }
}
