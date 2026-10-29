namespace Snake.Core.Tests;

public class HighScoreTableTests
{
    [Theory]
    [InlineData(Difficulty.Easy)]
    [InlineData(Difficulty.Normal)]
    [InlineData(Difficulty.Hard)]
    public void An_empty_table_has_no_records_for_any_difficulty_FR35(Difficulty difficulty)
    {
        Assert.Empty(HighScoreTable.Empty.For(difficulty));
    }

    [Fact]
    public void A_table_keeps_at_most_ten_records_per_difficulty_FR36()
    {
        var table = HighScores.TableOf(Difficulty.Normal, [.. Enumerable.Range(1, 15)]);

        Assert.Equal(HighScoreTable.MaxEntriesPerDifficulty, table.For(Difficulty.Normal).Count);
    }

    [Fact]
    public void A_full_table_drops_the_lowest_score_FR36_FR37()
    {
        var table = HighScores.TableOf(Difficulty.Normal, [.. Enumerable.Range(1, 15)]);

        Assert.Equal([15, 14, 13, 12, 11, 10, 9, 8, 7, 6], HighScores.ScoresOf(table, Difficulty.Normal));
    }

    [Fact]
    public void A_score_too_low_for_a_full_table_is_not_added_FR36_FR37()
    {
        var table = HighScores.TableOf(Difficulty.Normal, [.. Enumerable.Range(10, 10)]);

        table = table.Add(Difficulty.Normal, HighScores.Of(1));

        Assert.DoesNotContain(1, HighScores.ScoresOf(table, Difficulty.Normal));
    }

    [Fact]
    public void Records_are_sorted_by_score_descending_FR37()
    {
        var table = HighScores.TableOf(Difficulty.Hard, 30, 10, 50, 20);

        Assert.Equal([50, 30, 20, 10], HighScores.ScoresOf(table, Difficulty.Hard));
    }

    [Fact]
    public void On_an_equal_score_the_earlier_date_goes_first_FR37()
    {
        var later = HighScores.Of(100, "Поздний", day: 20);
        var earlier = HighScores.Of(100, "Ранний", day: 5);

        var table = HighScoreTable.Empty
            .Add(Difficulty.Normal, later)
            .Add(Difficulty.Normal, earlier);

        Assert.Equal([earlier, later], table.For(Difficulty.Normal));
    }

    [Fact]
    public void On_an_equal_score_and_date_the_record_added_first_stays_above_FR37()
    {
        var first = HighScores.Of(100, "Первый");
        var second = HighScores.Of(100, "Второй");

        var table = HighScoreTable.Empty
            .Add(Difficulty.Normal, first)
            .Add(Difficulty.Normal, second);

        Assert.Equal([first, second], table.For(Difficulty.Normal));
    }

    [Fact]
    public void Records_of_different_difficulties_do_not_mix_FR35()
    {
        var table = HighScoreTable.Empty
            .Add(Difficulty.Easy, HighScores.Of(10))
            .Add(Difficulty.Normal, HighScores.Of(20))
            .Add(Difficulty.Hard, HighScores.Of(30));

        Assert.Equal([10], HighScores.ScoresOf(table, Difficulty.Easy));
        Assert.Equal([20], HighScores.ScoresOf(table, Difficulty.Normal));
        Assert.Equal([30], HighScores.ScoresOf(table, Difficulty.Hard));
    }

    [Fact]
    public void Adding_a_record_leaves_the_previous_table_unchanged_FR35()
    {
        var table = HighScores.TableOf(Difficulty.Normal, 10);

        table.Add(Difficulty.Normal, HighScores.Of(20));

        Assert.Equal([10], HighScores.ScoresOf(table, Difficulty.Normal));
    }

    [Fact]
    public void A_table_built_from_loaded_records_is_sorted_and_trimmed_FR36_FR37()
    {
        var loaded = new Dictionary<Difficulty, IReadOnlyList<HighScore>>
        {
            [Difficulty.Easy] = [.. Enumerable.Range(1, 12).Select(score => HighScores.Of(score))]
        };

        var table = HighScoreTable.From(loaded);

        Assert.Equal([12, 11, 10, 9, 8, 7, 6, 5, 4, 3], HighScores.ScoresOf(table, Difficulty.Easy));
    }
}
