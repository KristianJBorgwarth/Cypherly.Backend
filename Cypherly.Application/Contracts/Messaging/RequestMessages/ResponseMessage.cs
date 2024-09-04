using Cypherly.Domain.ValueObjects;

namespace Cypherly.Application.Contracts.Messaging.RequestMessages;

public abstract class ResponseMessage(bool success, Error? error)
{
    public bool Success { get; init; } = success;
    public Error? Error { get; init; } = error;
}