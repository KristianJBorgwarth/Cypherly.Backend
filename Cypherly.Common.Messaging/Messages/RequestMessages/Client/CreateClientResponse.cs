namespace Cypherly.Common.Messaging.Messages.RequestMessages.Client;

public class CreateClientResponse(bool isSuccess = true, string? error = null) : ResponseMessage(isSuccess, error) { }