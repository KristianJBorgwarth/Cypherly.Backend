
using Cypherly.Application.Abstractions;

namespace Cypherly.Authentication.Application.Features.User.Commands.Delete;

public sealed record DeleteUserCommand : ICommandId
{
    public required Guid Id { get; init; }
}