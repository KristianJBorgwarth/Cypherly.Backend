using Cypherly.Common.Messaging.Enums;

namespace Cypherly.ChatServer.Application.Features.ChangeEvent;

public class ChangeEvent<T>
{
    /// <summary>
    /// Unique identifier for the event
    /// </summary>
    public Guid Id { get; private init; }

    /// <summary>
    /// The type of the event
    /// </summary>
    public ChangeEventType Type { get; private set; }

    /// <summary>
    /// A short, human-readable title for the event
    /// </summary>
    public string Title { get; private set; }

    /// <summary>
    /// Short description of the event
    /// </summary>
    public string Description { get; private set; }

    /// <summary>
    /// When the event occured
    /// </summary>
    public DateTime OccurredOn { get; private init; } = DateTime.UtcNow;

    /// <summary>
    /// Payload containing the data of the event
    /// </summary>
    public T Data { get; private set; }

}