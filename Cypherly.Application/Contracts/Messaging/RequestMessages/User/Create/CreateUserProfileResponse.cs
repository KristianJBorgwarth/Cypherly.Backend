using Cypherly.Domain.ValueObjects;

namespace Cypherly.Application.Contracts.Messaging.RequestMessages.User.Create;

public class CreateUserProfileResponse(bool isSuccess = true, Error? error = null) : ResponseMessage(isSuccess, error);