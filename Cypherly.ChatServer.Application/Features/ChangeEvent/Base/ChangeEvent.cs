﻿using Cypherly.Common.Messaging.Enums;

namespace Cypherly.ChatServer.Application.Features.ChangeEvent;

public class ChangeEvent(Guid id, ChangeEventType type, string title, string description, object data)
{
    /// <summary>
    /// Unique identifier for the event
    /// </summary>
    public Guid Id { get; private init; } = id;

    /// <summary>
    /// The type of the event
    /// </summary>
    public ChangeEventType Type { get; private set; } = type;

    /// <summary>
    /// A short, human-readable title for the event
    /// </summary>
    public string Title { get; private set; } = title;

    /// <summary>
    /// Short description of the event
    /// </summary>
    public string Description { get; private set; } = description;

    /// <summary>
    /// When the event occured
    /// </summary>
    public DateTime OccurredOn { get; private init; } = DateTime.UtcNow;

    /// <summary>
    /// Payload containing the data of the event
    /// </summary>
    public object Data { get; private set; } = data;
}