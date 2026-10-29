using System.Windows;
using Snake.Core;
using Snake.Wpf.ViewModels;

namespace Snake.Wpf;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var storage = new GameStorage(new AppDataDocumentStore());
        var window = new MainWindow(new ShellViewModel(storage, new Random()));

        MainWindow = window;
        window.Show();
    }
}
