namespace Cypherly.ChatServer.Application.Cache.Client;

public class ClientCacheDto
{
    public Guid ConnectionId { get; private init; }
    public string TransientId { get; private init; }

    private ClientCacheDto() { }

    public static ClientCacheDto Create(Domain.Aggregates.Client client, string transientId)
    {
        return new ClientCacheDto
        {
            ConnectionId = client.ConnectionId,
            TransientId = transientId,
        };
    }

    public static ClientCacheDto FromCache (Guid connectionId, string transientId)
    {
        return new ClientCacheDto
        {
            ConnectionId = connectionId,
            TransientId = transientId,
        };
    }
}