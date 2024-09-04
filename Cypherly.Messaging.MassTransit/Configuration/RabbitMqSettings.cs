﻿namespace Cypherly.Messaging.MassTransit.Configuration;

public sealed class RabbitMqSettings
{
    public required string Host { get; init; } = null!;
    public required string Username { get; init; } = null!;
    public required string Password { get; init; } = null!;
}