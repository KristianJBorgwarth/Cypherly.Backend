using Cypherly.Common.Messaging.Enums;

namespace Cypherly.Common.Messaging.Messages.PublishMessages;

public class OperationSuccededMessage(
    OperationType operationType,
    Guid correlationId,
    Guid? causationId = null)
    : BaseMessage(correlationId, causationId)
{
    public OperationType OperationType { get; private set; } = operationType;

}