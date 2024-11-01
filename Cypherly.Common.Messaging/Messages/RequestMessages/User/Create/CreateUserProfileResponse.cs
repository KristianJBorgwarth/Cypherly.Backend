namespace Cypherly.Common.Messaging.Messages.RequestMessages.User.Create;

public class CreateUserProfileResponse(bool isSuccess = true, string? error = null) : ResponseMessage(isSuccess, error);