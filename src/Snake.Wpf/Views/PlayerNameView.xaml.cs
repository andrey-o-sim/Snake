using System.Windows.Controls;

namespace Snake.Wpf.Views;

public partial class PlayerNameView : UserControl
{
    public PlayerNameView()
    {
        InitializeComponent();
        Loaded += (_, _) =>
        {
            NameBox.Focus();
            NameBox.SelectAll();
        };
    }
}
