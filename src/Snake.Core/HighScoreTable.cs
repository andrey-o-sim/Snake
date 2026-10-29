namespace Snake.Core;

public sealed class HighScoreTable
{
    public const int MaxEntriesPerDifficulty = 10;

    private static readonly IReadOnlyList<HighScore> NoEntries = [];

    private readonly IReadOnlyDictionary<Difficulty, IReadOnlyList<HighScore>> entries;

    private HighScoreTable(IReadOnlyDictionary<Difficulty, IReadOnlyList<HighScore>> entries) =>
        this.entries = entries;

    public static HighScoreTable Empty { get; } = new(new Dictionary<Difficulty, IReadOnlyList<HighScore>>());

    public IReadOnlyDictionary<Difficulty, IReadOnlyList<HighScore>> ByDifficulty => entries;

    public IReadOnlyList<HighScore> For(Difficulty difficulty) =>
        entries.TryGetValue(difficulty, out var found) ? found : NoEntries;

    public HighScoreTable Add(Difficulty difficulty, HighScore highScore)
    {
        var updated = new Dictionary<Difficulty, IReadOnlyList<HighScore>>(entries)
        {
            [difficulty] = Ranked([.. For(difficulty), highScore])
        };

        return new HighScoreTable(updated);
    }

    public static HighScoreTable From(IReadOnlyDictionary<Difficulty, IReadOnlyList<HighScore>> entries)
    {
        var table = Empty;

        foreach (var (difficulty, loaded) in entries)
        {
            if (loaded is null)
            {
                continue;
            }

            foreach (var entry in loaded)
            {
                if (entry is not null)
                {
                    table = table.Add(difficulty, entry);
                }
            }
        }

        return table;
    }

    private static IReadOnlyList<HighScore> Ranked(IEnumerable<HighScore> entries) =>
    [
        .. entries
            .OrderByDescending(entry => entry.Score)
            .ThenBy(entry => entry.Date)
            .Take(MaxEntriesPerDifficulty)
    ];
}
