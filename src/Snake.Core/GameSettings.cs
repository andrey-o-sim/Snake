namespace Snake.Core;

public sealed record GameSettings(PlayerName PlayerName, Difficulty Difficulty)
{
    private const Difficulty DefaultDifficulty = Difficulty.Normal;

    public static GameSettings Default { get; } = new(PlayerName.Default, DefaultDifficulty);
}
