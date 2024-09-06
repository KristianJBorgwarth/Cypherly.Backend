﻿namespace Cypherly.Application.Contracts.Messaging.RequestMessages.User.Create;

public sealed class CreateUserProfileRequest(Guid userId, string username) : RequestMessage
{
    public Guid UserId { get; private set; } = userId;
    public string Username { get; private set; } = username;
}