namespace Cypherly.Common.Messaging.Messages.RequestMessages;

public abstract class ResponseMessage(bool isSuccess, string? error)
{
    public bool IsSuccess { get; init; } = isSuccess;
    public string? Error { get; init; } = error;
}