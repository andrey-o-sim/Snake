using System.Windows.Input;
using Snake.Wpf.Mvvm;

namespace Snake.Wpf.ViewModels;

public sealed class MenuViewModel : ObservableObject
{
    private readonly ShellViewModel shell;

    private DifficultyOption selectedDifficulty;

    public MenuViewModel(ShellViewModel shell)
    {
        this.shell = shell;

        selectedDifficulty = DifficultyOption.Of(shell.Difficulty);
        StartCommand = new RelayCommand(shell.StartGame);
        HighScoresCommand = new RelayCommand(shell.ShowHighScores);
        ChangeNameCommand = new RelayCommand(shell.ChangePlayerName);
        ExitCommand = new RelayCommand(shell.Exit);
    }

    public string Greeting => $"Игрок: {shell.PlayerName}";

    public IReadOnlyList<DifficultyOption> Difficulties => DifficultyOption.All;

    public DifficultyOption SelectedDifficulty
    {
        get => selectedDifficulty;
        set
        {
            Set(ref selectedDifficulty, value);
            shell.SelectDifficulty(value.Difficulty);
        }
    }

    public ICommand StartCommand { get; }

    public ICommand HighScoresCommand { get; }

    public ICommand ChangeNameCommand { get; }

    public ICommand ExitCommand { get; }
}
