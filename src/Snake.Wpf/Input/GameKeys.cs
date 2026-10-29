using System.Windows.Input;
using Snake.Core;

namespace Snake.Wpf.Input;

public static class GameKeys
{
    public static Direction? DirectionOf(Key key) => key switch
    {
        Key.Up or Key.W => Direction.Up,
        Key.Down or Key.S => Direction.Down,
        Key.Left or Key.A => Direction.Left,
        Key.Right or Key.D => Direction.Right,
        _ => null
    };

    public static bool IsPause(Key key) => key is Key.Space or Key.P;

    public static bool IsNewGame(Key key) => key is Key.Enter;

    public static bool IsBack(Key key) => key is Key.Escape;
}
