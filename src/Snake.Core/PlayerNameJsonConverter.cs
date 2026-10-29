using System.Text.Json;
using System.Text.Json.Serialization;

namespace Snake.Core;

internal sealed class PlayerNameJsonConverter : JsonConverter<PlayerName>
{
    public override PlayerName Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Expected a string for {nameof(PlayerName)}, got {reader.TokenType}.");
        }

        return PlayerName.From(reader.GetString());
    }

    public override void Write(Utf8JsonWriter writer, PlayerName value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.Value);
}
