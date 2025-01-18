using System.Reflection.Metadata;
using Cypherly.ChatServer.Domain.Entities;
using Cypherly.Domain.Common;
// ReSharper disable ConvertToPrimaryConstructor

namespace Cypherly.ChatServer.Domain.Aggregates;

public class Client : AggregateRoot
{
    public Guid ConnectionId { get; init; }
    public string? TransientId { get; private set; }
    private Client () : base(Guid.Empty) { } // For EF Core
    
    private List<Message> _messages = [];
    public virtual IReadOnlyCollection<Message> Messages => _messages;
    
    public Client(Guid id, Guid connectionId) : base(id)
    {
        ConnectionId = connectionId;
    }
    
    public void SetTransientId(string transientId) => TransientId = transientId;
}