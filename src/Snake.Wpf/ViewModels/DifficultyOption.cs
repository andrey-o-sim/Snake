using Snake.Core;

namespace Snake.Wpf.ViewModels;

public sealed class DifficultyOption
{
    private const string WrapWall = "проход сквозь стену";
    private const string DeadlyWall = "стена убивает";

    private DifficultyOption(Difficulty difficulty, string title)
    {
        var rules = DifficultyRules.For(difficulty);

        Difficulty = difficulty;
        Title = title;
        Rule = $"{rules.TickInterval.TotalMilliseconds:0} мс · ×{rules.ScoreMultiplier} · " +
            (rules.WrapsAtWall ? WrapWall : DeadlyWall);
    }

    public static IReadOnlyList<DifficultyOption> All { get; } =
    [
        new(Difficulty.Easy, "Лёгкий"),
        new(Difficulty.Normal, "Обычный"),
        new(Difficulty.Hard, "Сложный")
    ];

    public Difficulty Difficulty { get; }

    public string Title { get; }

    public string Rule { get; }

    public static DifficultyOption Of(Difficulty difficulty) =>
        All.Single(option => option.Difficulty == difficulty);
}
