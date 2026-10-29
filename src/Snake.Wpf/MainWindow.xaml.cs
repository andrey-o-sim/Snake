using System.Windows;
using System.Windows.Input;
using Snake.Wpf.Input;
using Snake.Wpf.ViewModels;

namespace Snake.Wpf;

public partial class MainWindow : Window
{
    private readonly ShellViewModel shell;

    public MainWindow(ShellViewModel shell)
    {
        this.shell = shell;

        InitializeComponent();
        DataContext = shell;
    }

    private void OnKeyDown(object sender, KeyEventArgs e)
    {
        if (shell.Screen is IKeyboardScreen screen && screen.HandleKey(e.Key))
        {
            e.Handled = true;
        }
    }

    private void OnDeactivated(object? sender, EventArgs e) => shell.PauseOnFocusLoss();
}
