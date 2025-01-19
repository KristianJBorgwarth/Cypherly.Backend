using Cypherly.ChatServer.Domain.Enums;
using Cypherly.Domain.Common;

namespace Cypherly.ChatServer.Domain.Entities;

public class ChatMessage : Entity
{
    public Guid MessageCorrelator { get; init; }
    public Guid RecipientId { get; init; }
    public Guid SenderId { get; init; }
    public string Content { get; init; } = null!;
    public MessageStatus Status { get; private set; }
    
    private ChatMessage () : base(Guid.Empty) { } // For EF Core
    
    public ChatMessage(Guid id, Guid messageCorrelator, Guid recipientId, Guid senderId, string content) : base(id)
    {
        MessageCorrelator = messageCorrelator;
        RecipientId = recipientId;
        SenderId = senderId;
        Content = content;
        Status = MessageStatus.Sent;
    }
    
    public void MarkAsDelivered()
    {
        Status = MessageStatus.Delivered;
    }
    
    public void MarkAsRead()
    {
        Status = MessageStatus.Read;
    }
}