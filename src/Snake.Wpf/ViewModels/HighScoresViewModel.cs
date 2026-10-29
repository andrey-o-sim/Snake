using System.Windows.Input;
using Snake.Core;
using Snake.Wpf.Input;
using Snake.Wpf.Mvvm;

namespace Snake.Wpf.ViewModels;

public sealed class HighScoresViewModel : ObservableObject, IKeyboardScreen
{
    private readonly ShellViewModel shell;
    private readonly HighScoreTable highScores;

    private DifficultyOption selectedDifficulty;

    public HighScoresViewModel(ShellViewModel shell, HighScoreTable highScores, Difficulty difficulty)
    {
        this.shell = shell;
        this.highScores = highScores;

        selectedDifficulty = DifficultyOption.Of(difficulty);
        BackCommand = new RelayCommand(shell.ShowMenu);
    }

    public IReadOnlyList<DifficultyOption> Difficulties => DifficultyOption.All;

    public DifficultyOption SelectedDifficulty
    {
        get => selectedDifficulty;
        set
        {
            Set(ref selectedDifficulty, value);
            Notify(nameof(Rows));
            Notify(nameof(IsEmpty));
        }
    }

    public IReadOnlyList<HighScoreRow> Rows =>
    [
        .. highScores
            .For(selectedDifficulty.Difficulty)
            .Select((highScore, index) => new HighScoreRow(index + 1, highScore))
    ];

    public bool IsEmpty => Rows.Count == 0;

    public ICommand BackCommand { get; }

    public bool HandleKey(Key key)
    {
        if (!GameKeys.IsBack(key))
        {
            return false;
        }

        shell.ShowMenu();
        return true;
    }
}
