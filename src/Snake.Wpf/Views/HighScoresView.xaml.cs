using System.Windows.Controls;

namespace Snake.Wpf.Views;

public partial class HighScoresView : UserControl
{
    public HighScoresView()
    {
        InitializeComponent();
        Loaded += (_, _) => DifficultyList.Focus();
    }
}
