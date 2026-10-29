using System.Windows.Input;
using System.Windows.Threading;
using Snake.Core;
using Snake.Wpf.Input;
using Snake.Wpf.Mvvm;

namespace Snake.Wpf.ViewModels;

public sealed class GameViewModel : ObservableObject, IKeyboardScreen, IDisposable
{
    private const string PauseTitle = "Пауза";
    private const string GameOverTitle = "Игра окончена";
    private const string WonTitle = "Поле заполнено. Победа!";
    private const string PauseHint = "Space или P — продолжить · Esc — в меню";
    private const string FinishedHint = "Enter — новая партия · Esc — в меню";

    private readonly ShellViewModel shell;
    private readonly GameStorage storage;
    private readonly PlayerName playerName;
    private readonly DispatcherTimer timer;

    private Game game;
    private bool paused;

    public GameViewModel(
        ShellViewModel shell,
        GameStorage storage,
        Random random,
        Difficulty difficulty,
        PlayerName playerName)
    {
        this.shell = shell;
        this.storage = storage;
        this.playerName = playerName;

        game = Game.Start(difficulty, random);
        timer = new DispatcherTimer { Interval = game.TickInterval };
        timer.Tick += OnTick;
        timer.Start();
    }

    public Game Game => game;

    public string PlayerName => playerName.Value;

    public string DifficultyTitle => DifficultyOption.Of(game.Difficulty).Title;

    public int Score => game.Score;

    public bool IsFinished => game.Status != GameStatus.Running;

    public bool IsOverlayVisible => paused || IsFinished;

    public string OverlayTitle => game.Status switch
    {
        GameStatus.Won => WonTitle,
        GameStatus.GameOver => GameOverTitle,
        _ => PauseTitle
    };

    public string OverlayHint => IsFinished ? FinishedHint : PauseHint;

    public string OverlayScore => IsFinished ? $"Счёт: {game.Score}" : string.Empty;

    public bool HandleKey(Key key)
    {
        if (GameKeys.IsBack(key))
        {
            shell.ShowMenu();
            return true;
        }

        if (IsFinished)
        {
            return GameKeys.IsNewGame(key) && StartNewGame();
        }

        if (GameKeys.IsPause(key))
        {
            TogglePause();
            return true;
        }

        if (paused)
        {
            return false;
        }

        var direction = GameKeys.DirectionOf(key);

        if (direction is null)
        {
            return false;
        }

        game = game.Turn(direction.Value);
        return true;
    }

    public void PauseOnFocusLoss()
    {
        if (IsFinished || paused)
        {
            return;
        }

        TogglePause();
    }

    public void Dispose()
    {
        timer.Stop();
        timer.Tick -= OnTick;
    }

    private bool StartNewGame()
    {
        shell.StartGame();
        return true;
    }

    private void TogglePause()
    {
        paused = !paused;

        if (paused)
        {
            timer.Stop();
        }
        else
        {
            timer.Start();
        }

        NotifyScreenChanged();
    }

    private void OnTick(object? sender, EventArgs e)
    {
        game = game.Tick();

        if (IsFinished)
        {
            timer.Stop();
            SaveHighScore();
        }

        NotifyScreenChanged();
    }

    private void SaveHighScore()
    {
        var highScore = new HighScore(playerName, game.Score, DateOnly.FromDateTime(DateTime.Now));

        storage.SaveHighScores(storage.LoadHighScores().Add(game.Difficulty, highScore));
    }

    private void NotifyScreenChanged()
    {
        Notify(nameof(Game));
        Notify(nameof(Score));
        Notify(nameof(IsFinished));
        Notify(nameof(IsOverlayVisible));
        Notify(nameof(OverlayTitle));
        Notify(nameof(OverlayHint));
        Notify(nameof(OverlayScore));
    }
}
