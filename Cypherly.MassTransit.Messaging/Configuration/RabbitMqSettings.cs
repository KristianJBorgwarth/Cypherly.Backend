﻿namespace Cypherly.MassTransit.Messaging.Configuration;

public sealed class RabbitMqSettings
{
    public required string Host { get; init; } = null!;
    public required string Username { get; init; } = null!;
    public required string Password { get; init; } = null!;
}