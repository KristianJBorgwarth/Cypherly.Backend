namespace Cypherly.Common.Messaging.Messages.RequestMessages.Client;

public class CreateClientResponse(bool isSuccess, string? error = null) : ResponseMessage(isSuccess, error) { }