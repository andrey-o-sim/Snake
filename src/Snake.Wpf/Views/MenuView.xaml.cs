using System.Windows.Controls;

namespace Snake.Wpf.Views;

public partial class MenuView : UserControl
{
    public MenuView()
    {
        InitializeComponent();
        Loaded += (_, _) => StartButton.Focus();
    }
}
