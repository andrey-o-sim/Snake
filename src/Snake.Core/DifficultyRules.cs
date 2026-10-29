namespace Snake.Core;

public sealed record DifficultyRules(TimeSpan TickInterval, int ScoreMultiplier, bool WrapsAtWall)
{
    private const int EasyTickIntervalMilliseconds = 200;
    private const int NormalTickIntervalMilliseconds = 130;
    private const int HardTickIntervalMilliseconds = 90;

    private const int EasyScoreMultiplier = 1;
    private const int NormalScoreMultiplier = 2;
    private const int HardScoreMultiplier = 3;

    public static DifficultyRules For(Difficulty difficulty) => difficulty switch
    {
        Difficulty.Easy => new DifficultyRules(
            TimeSpan.FromMilliseconds(EasyTickIntervalMilliseconds), EasyScoreMultiplier, WrapsAtWall: true),
        Difficulty.Normal => new DifficultyRules(
            TimeSpan.FromMilliseconds(NormalTickIntervalMilliseconds), NormalScoreMultiplier, WrapsAtWall: false),
        Difficulty.Hard => new DifficultyRules(
            TimeSpan.FromMilliseconds(HardTickIntervalMilliseconds), HardScoreMultiplier, WrapsAtWall: false),
        _ => throw new ArgumentOutOfRangeException(nameof(difficulty))
    };
}
