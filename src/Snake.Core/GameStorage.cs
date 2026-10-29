using System.Text.Json;
using System.Text.Json.Serialization;

namespace Snake.Core;

public sealed class GameStorage(IDocumentStore documents)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter(), new PlayerNameJsonConverter() }
    };

    public bool HasSettings => documents.Exists(StoredDocument.Settings);

    public HighScoreTable LoadHighScores()
    {
        var loaded = ReadOrNull<Dictionary<Difficulty, IReadOnlyList<HighScore>>>(StoredDocument.HighScores);

        return loaded is null ? HighScoreTable.Empty : HighScoreTable.From(loaded);
    }

    public void SaveHighScores(HighScoreTable highScores) =>
        Write(StoredDocument.HighScores, highScores.ByDifficulty);

    public GameSettings LoadSettings() =>
        ReadOrNull<GameSettings>(StoredDocument.Settings) ?? GameSettings.Default;

    public void SaveSettings(GameSettings settings) => Write(StoredDocument.Settings, settings);

    private T? ReadOrNull<T>(StoredDocument document)
        where T : class
    {
        try
        {
            var content = documents.Read(document);

            return content is null ? null : JsonSerializer.Deserialize<T>(content, JsonOptions);
        }
        catch (JsonException)
        {
            return null;
        }
        catch (IOException)
        {
            return null;
        }
        catch (UnauthorizedAccessException)
        {
            return null;
        }
    }

    private void Write<T>(StoredDocument document, T content) =>
        documents.Write(document, JsonSerializer.Serialize(content, JsonOptions));
}
