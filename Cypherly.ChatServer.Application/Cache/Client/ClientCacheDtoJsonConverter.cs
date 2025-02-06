using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cypherly.ChatServer.Application.Cache.Client;

public class ClientCacheDtoJsonConverter : JsonConverter<ClientCacheDto>
{
    public override ClientCacheDto Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var jsonObject = JsonDocument.ParseValue(ref reader).RootElement;
        var connectionId = jsonObject.GetProperty("ConnectionId").GetGuid();
        var transientId = jsonObject.GetProperty("TransientId").GetString();

        return ClientCacheDto.FromCache(connectionId, transientId);
    }

    public override void Write(Utf8JsonWriter writer, ClientCacheDto value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString("ConnectionId", value.ConnectionId);
        writer.WriteString("TransientId", value.TransientId);
        writer.WriteEndObject();
    }
}