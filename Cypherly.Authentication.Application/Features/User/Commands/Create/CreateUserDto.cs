﻿namespace Cypherly.Authentication.Application.Features.User.Commands.Create;

public sealed record CreateUserDto
{
    public required Guid Id { get; init; }
    public required string Email { get; init; }

    public static CreateUserDto Map(Domain.Aggregates.User user)
    {
        return new CreateUserDto
        {
            Id = user.Id,
            Email = user.Email.Address,
        };
    }
}