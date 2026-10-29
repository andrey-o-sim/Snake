using System.Windows.Input;

namespace Snake.Wpf.Input;

public interface IKeyboardScreen
{
    bool HandleKey(Key key);
}
