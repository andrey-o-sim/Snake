using System.Windows.Input;
using Snake.Core;
using Snake.Wpf.Input;
using Snake.Wpf.Mvvm;

namespace Snake.Wpf.ViewModels;

public sealed class PlayerNameViewModel : ObservableObject, IKeyboardScreen
{
    private readonly Action<PlayerName> confirm;
    private readonly Action cancel;

    private string text;

    public PlayerNameViewModel(PlayerName current, bool canCancel, Action<PlayerName> confirm, Action cancel)
    {
        this.confirm = confirm;
        this.cancel = cancel;

        text = current.Value;
        CanCancel = canCancel;
        ConfirmCommand = new RelayCommand(Confirm);
        CancelCommand = new RelayCommand(cancel);
    }

    public string Title => CanCancel ? "Имя игрока" : "Добро пожаловать";

    public string Text
    {
        get => text;
        set => Set(ref text, value);
    }

    public int MaxLength => PlayerName.MaxLength;

    public string Hint => $"До {PlayerName.MaxLength} символов. Пустое имя станет «{PlayerName.Default}».";

    public bool CanCancel { get; }

    public ICommand ConfirmCommand { get; }

    public ICommand CancelCommand { get; }

    public bool HandleKey(Key key)
    {
        if (GameKeys.IsNewGame(key))
        {
            Confirm();
            return true;
        }

        if (GameKeys.IsBack(key) && CanCancel)
        {
            cancel();
            return true;
        }

        return false;
    }

    private void Confirm() => confirm(PlayerName.From(Text));
}
