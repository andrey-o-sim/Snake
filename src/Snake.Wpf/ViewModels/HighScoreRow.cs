using System.Globalization;
using Snake.Core;

namespace Snake.Wpf.ViewModels;

public sealed class HighScoreRow(int rank, HighScore highScore)
{
    private const string DateFormat = "dd.MM.yyyy";

    public int Rank { get; } = rank;

    public string PlayerName { get; } = highScore.PlayerName.Value;

    public int Score { get; } = highScore.Score;

    public string Date { get; } = highScore.Date.ToString(DateFormat, CultureInfo.InvariantCulture);
}
