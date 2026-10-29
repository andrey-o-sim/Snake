namespace Snake.Core.Tests;

public class GameStorageTests
{
    private readonly InMemoryDocumentStore documents = new();

    [Fact]
    public void High_scores_survive_a_save_and_a_load_FR38_FR39()
    {
        var storage = new GameStorage(documents);
        var saved = HighScoreTable.Empty
            .Add(Difficulty.Easy, HighScores.Of(10, "Аня"))
            .Add(Difficulty.Hard, HighScores.Of(90, "Борис", day: 7));

        storage.SaveHighScores(saved);
        var loaded = new GameStorage(documents).LoadHighScores();

        Assert.Equal(saved.For(Difficulty.Easy), loaded.For(Difficulty.Easy));
        Assert.Equal(saved.For(Difficulty.Hard), loaded.For(Difficulty.Hard));
    }

    [Fact]
    public void Settings_survive_a_save_and_a_load_FR34_FR39_FR42()
    {
        var storage = new GameStorage(documents);
        var saved = new GameSettings(PlayerName.From("Аня"), Difficulty.Hard);

        storage.SaveSettings(saved);

        Assert.Equal(saved, new GameStorage(documents).LoadSettings());
    }

    [Fact]
    public void Difficulty_is_written_as_a_string_FR39()
    {
        new GameStorage(documents).SaveSettings(new GameSettings(PlayerName.Default, Difficulty.Hard));

        Assert.Contains("\"Hard\"", documents.Content(StoredDocument.Settings));
    }

    [Fact]
    public void High_scores_are_grouped_by_the_difficulty_name_FR35_FR39()
    {
        new GameStorage(documents).SaveHighScores(HighScores.TableOf(Difficulty.Easy, 10));

        Assert.Contains("\"Easy\"", documents.Content(StoredDocument.HighScores));
    }

    [Fact]
    public void A_missing_high_scores_document_gives_an_empty_table_FR41()
    {
        var table = new GameStorage(documents).LoadHighScores();

        Assert.Empty(table.For(Difficulty.Normal));
    }

    [Fact]
    public void A_missing_settings_document_gives_the_default_settings_FR41()
    {
        Assert.Equal(GameSettings.Default, new GameStorage(documents).LoadSettings());
    }

    [Theory]
    [InlineData("{ broken")]
    [InlineData("[]")]
    [InlineData("{ \"Normal\": 42 }")]
    [InlineData("{ \"Insane\": [] }")]
    public void A_corrupted_high_scores_document_gives_an_empty_table_without_an_exception_FR41(string content)
    {
        documents.Write(StoredDocument.HighScores, content);

        var table = new GameStorage(documents).LoadHighScores();

        Assert.All(AllDifficulties, difficulty => Assert.Empty(table.For(difficulty)));
    }

    [Theory]
    [InlineData("{ broken")]
    [InlineData("not json at all")]
    [InlineData("{ \"PlayerName\": 42, \"Difficulty\": \"Easy\" }")]
    [InlineData("{ \"PlayerName\": \"Аня\", \"Difficulty\": \"Insane\" }")]
    public void A_corrupted_settings_document_gives_the_default_settings_without_an_exception_FR41(string content)
    {
        documents.Write(StoredDocument.Settings, content);

        Assert.Equal(GameSettings.Default, new GameStorage(documents).LoadSettings());
    }

    [Fact]
    public void A_document_that_cannot_be_read_gives_an_empty_table_FR41()
    {
        var storage = new GameStorage(new UnreadableDocumentStore());

        Assert.Empty(storage.LoadHighScores().For(Difficulty.Normal));
    }

    [Fact]
    public void A_document_that_cannot_be_read_gives_the_default_settings_FR41()
    {
        var storage = new GameStorage(new UnreadableDocumentStore());

        Assert.Equal(GameSettings.Default, storage.LoadSettings());
    }

    [Fact]
    public void A_document_holding_only_null_gives_the_default_settings_FR41()
    {
        documents.Write(StoredDocument.Settings, "null");

        Assert.Equal(GameSettings.Default, new GameStorage(documents).LoadSettings());
    }

    [Fact]
    public void More_than_ten_records_in_a_document_are_trimmed_on_load_FR36()
    {
        documents.Write(StoredDocument.HighScores, HighScoresJson(Enumerable.Range(1, 14)));

        var table = new GameStorage(documents).LoadHighScores();

        Assert.Equal([14, 13, 12, 11, 10, 9, 8, 7, 6, 5], HighScores.ScoresOf(table, Difficulty.Normal));
    }

    [Fact]
    public void Unsorted_records_in_a_document_are_sorted_on_load_FR37()
    {
        documents.Write(StoredDocument.HighScores, HighScoresJson([30, 70, 50]));

        var table = new GameStorage(documents).LoadHighScores();

        Assert.Equal([70, 50, 30], HighScores.ScoresOf(table, Difficulty.Normal));
    }

    [Fact]
    public void A_name_longer_than_twenty_characters_in_a_document_is_cut_on_load_FR44()
    {
        documents.Write(
            StoredDocument.Settings,
            $"{{ \"PlayerName\": \"{new string('a', 30)}\", \"Difficulty\": \"Easy\" }}");

        var settings = new GameStorage(documents).LoadSettings();

        Assert.Equal(new string('a', PlayerName.MaxLength), settings.PlayerName.Value);
    }

    [Fact]
    public void An_empty_name_in_a_document_becomes_the_default_name_FR44()
    {
        documents.Write(StoredDocument.Settings, "{ \"PlayerName\": \"\", \"Difficulty\": \"Easy\" }");

        var settings = new GameStorage(documents).LoadSettings();

        Assert.Equal(PlayerName.Default, settings.PlayerName);
    }

    [Fact]
    public void Settings_are_absent_before_the_first_save_FR42()
    {
        Assert.False(new GameStorage(documents).HasSettings);
    }

    [Fact]
    public void Settings_are_present_after_a_save_FR42()
    {
        var storage = new GameStorage(documents);

        storage.SaveSettings(GameSettings.Default);

        Assert.True(storage.HasSettings);
    }

    private static IEnumerable<Difficulty> AllDifficulties => Enum.GetValues<Difficulty>();

    private static string HighScoresJson(IEnumerable<int> scores)
    {
        var records = scores.Select(score =>
            $"{{ \"PlayerName\": \"Аня\", \"Score\": {score}, \"Date\": \"2026-01-01\" }}");

        return $"{{ \"Normal\": [ {string.Join(", ", records)} ] }}";
    }
}
