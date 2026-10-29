using System.Windows;
using Snake.Core;
using Snake.Wpf.Mvvm;

namespace Snake.Wpf.ViewModels;

public sealed class ShellViewModel : ObservableObject
{
    private readonly GameStorage storage;
    private readonly Random random;

    private GameSettings settings;
    private object screen;

    public ShellViewModel(GameStorage storage, Random random)
    {
        this.storage = storage;
        this.random = random;

        settings = storage.LoadSettings();
        screen = storage.HasSettings ? new MenuViewModel(this) : NameEntry(canCancel: false);
    }

    public object Screen
    {
        get => screen;
        private set
        {
            (screen as IDisposable)?.Dispose();
            Set(ref screen, value);
        }
    }

    public PlayerName PlayerName => settings.PlayerName;

    public Difficulty Difficulty => settings.Difficulty;

    public void ShowMenu() => Screen = new MenuViewModel(this);

    public void StartGame() =>
        Screen = new GameViewModel(this, storage, random, settings.Difficulty, settings.PlayerName);

    public void ShowHighScores() =>
        Screen = new HighScoresViewModel(this, storage.LoadHighScores(), settings.Difficulty);

    public void ChangePlayerName() => Screen = NameEntry(canCancel: true);

    public void SelectDifficulty(Difficulty difficulty) => Save(settings with { Difficulty = difficulty });

    public void Exit() => Application.Current.Shutdown();

    public void PauseOnFocusLoss() => (screen as GameViewModel)?.PauseOnFocusLoss();

    private PlayerNameViewModel NameEntry(bool canCancel) =>
        new(settings.PlayerName, canCancel, ApplyPlayerName, ShowMenu);

    private void ApplyPlayerName(PlayerName playerName)
    {
        Save(settings with { PlayerName = playerName });
        ShowMenu();
    }

    private void Save(GameSettings updated)
    {
        settings = updated;
        storage.SaveSettings(settings);
    }
}
