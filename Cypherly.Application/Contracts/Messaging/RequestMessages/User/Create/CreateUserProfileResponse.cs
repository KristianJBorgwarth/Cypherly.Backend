using Cypherly.Domain.ValueObjects;

namespace Cypherly.Application.Contracts.Messaging.RequestMessages.User.Create;

public class CreateUserProfileResponse(bool success, Error? error) : ResponseMessage(success, error);