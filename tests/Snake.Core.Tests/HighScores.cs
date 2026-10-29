namespace Snake.Core.Tests;

internal static class HighScores
{
    private const int Year = 2026;
    private const int Month = 1;

    internal static HighScore Of(int score, string name = "Аня", int day = 1) =>
        new(PlayerName.From(name), score, new DateOnly(Year, Month, day));

    internal static HighScoreTable TableOf(Difficulty difficulty, params int[] scores)
    {
        var table = HighScoreTable.Empty;

        foreach (var score in scores)
        {
            table = table.Add(difficulty, Of(score));
        }

        return table;
    }

    internal static IReadOnlyList<int> ScoresOf(HighScoreTable table, Difficulty difficulty) =>
        [.. table.For(difficulty).Select(entry => entry.Score)];
}
