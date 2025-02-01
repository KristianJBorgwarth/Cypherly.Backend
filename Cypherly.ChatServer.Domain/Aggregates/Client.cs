using System.Reflection.Metadata;
using Cypherly.ChatServer.Domain.Entities;
using Cypherly.Domain.Common;
// ReSharper disable ConvertToPrimaryConstructor
// ReSharper disable ClassWithVirtualMembersNeverInherited.Global

namespace Cypherly.ChatServer.Domain.Aggregates;

public class Client : AggregateRoot
{
    /// <summary>
    /// ConnectionId is used send msg between clients and serves as a stable identifier for the SignalR connection.
    /// </summary>
    public Guid ConnectionId { get; init; }

    /// <summary>
    /// TransientId is used to store the SignalR connectionId for caching purposes.
    /// Will not be persisted to the database.
    /// </summary>
    public string? TransientId { get; private set; }
    private Client () : base(Guid.Empty) { } // For EF Core

    private List<ChatMessage> _chatMessages = [];
    public IReadOnlyCollection<ChatMessage> ChatMessages => _chatMessages;

    public Client(Guid id, Guid connectionId) : base(id)
    {
        ConnectionId = connectionId;
    }

    public void SetTransientId(string transientId) => TransientId = transientId;
}