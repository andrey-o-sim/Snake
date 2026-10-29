namespace Snake.Core;

public sealed class AppDataDocumentStore : IDocumentStore
{
    private const string FolderName = "Snake";
    private const string HighScoresFileName = "highscores.json";
    private const string SettingsFileName = "settings.json";

    public static string Folder =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), FolderName);

    public static string PathOf(StoredDocument document) => Path.Combine(Folder, FileNameOf(document));

    public bool Exists(StoredDocument document) => File.Exists(PathOf(document));

    public string? Read(StoredDocument document)
    {
        var path = PathOf(document);

        return File.Exists(path) ? File.ReadAllText(path) : null;
    }

    public void Write(StoredDocument document, string content)
    {
        Directory.CreateDirectory(Folder);
        File.WriteAllText(PathOf(document), content);
    }

    private static string FileNameOf(StoredDocument document) => document switch
    {
        StoredDocument.HighScores => HighScoresFileName,
        StoredDocument.Settings => SettingsFileName,
        _ => throw new ArgumentOutOfRangeException(nameof(document))
    };
}
