using Cypherly.Domain.ValueObjects;

namespace Cypherly.Application.Contracts.Messaging.RequestMessages;

public abstract class ResponseMessage(bool isSuccess, Error? error)
{
    public bool IsSuccess { get; init; } = isSuccess;
    public Error? Error { get; init; } = error;
}