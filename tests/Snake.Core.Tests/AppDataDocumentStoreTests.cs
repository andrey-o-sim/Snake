namespace Snake.Core.Tests;

public class AppDataDocumentStoreTests
{
    [Fact]
    public void Documents_live_in_the_user_profile_folder_FR40()
    {
        var expected = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Snake");

        Assert.Equal(expected, AppDataDocumentStore.Folder);
    }

    [Theory]
    [InlineData(StoredDocument.HighScores)]
    [InlineData(StoredDocument.Settings)]
    public void Every_document_is_a_json_file_inside_that_folder_FR39_FR40(StoredDocument document)
    {
        var path = AppDataDocumentStore.PathOf(document);

        Assert.Equal(AppDataDocumentStore.Folder, Path.GetDirectoryName(path));
        Assert.Equal(".json", Path.GetExtension(path));
    }

    [Fact]
    public void High_scores_and_settings_are_kept_in_different_files_FR40()
    {
        Assert.NotEqual(
            AppDataDocumentStore.PathOf(StoredDocument.HighScores),
            AppDataDocumentStore.PathOf(StoredDocument.Settings));
    }
}
