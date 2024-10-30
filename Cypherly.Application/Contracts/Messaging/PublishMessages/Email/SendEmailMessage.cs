namespace Cypherly.Application.Contracts.Messaging.PublishMessages.Email;

public sealed class SendEmailMessage : BaseMessage
{
    public string To { get; private set; }
    public string Subject { get; private set; }
    public string Body { get; private set; }

    /// <inheritdoc />
    public SendEmailMessage(string to, string subject, string body, Guid correlationId, Guid? causationId = null) : base(correlationId, causationId)
    {
        To = to;
        Subject = subject;
        Body = body;
    }
}