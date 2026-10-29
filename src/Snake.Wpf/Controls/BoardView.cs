using System.Windows;
using System.Windows.Media;
using Snake.Core;

namespace Snake.Wpf.Controls;

public sealed class BoardView : FrameworkElement
{
    public static readonly DependencyProperty GameProperty = DependencyProperty.Register(
        nameof(Game),
        typeof(Game),
        typeof(BoardView),
        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

    private const double MinimumCellSize = 4;
    private const double WallThickness = 3;
    private const double GridThickness = 1;
    private const double SegmentInset = 0.07;
    private const double BodyCornerRatio = 0.22;
    private const double HeadCornerRatio = 0.38;
    private const double FoodRadiusRatio = 0.34;
    private const double FoodCoreRadiusRatio = 0.15;
    private const double EyeRadiusRatio = 0.09;
    private const double EyeForwardRatio = 0.18;
    private const double EyeSideRatio = 0.17;

    private static readonly Brush FieldBrush = Frozen(Color.FromRgb(0x0C, 0x10, 0x16));
    private static readonly Brush BodyBrush = Frozen(Color.FromRgb(0x2E, 0x8B, 0x57));
    private static readonly Brush HeadBrush = Frozen(Color.FromRgb(0xA7, 0xE8, 0xB8));
    private static readonly Brush EyeBrush = Frozen(Color.FromRgb(0x0C, 0x10, 0x16));
    private static readonly Brush FoodBrush = Frozen(Color.FromRgb(0xFF, 0x8A, 0x5B));

    private static readonly Pen GridPen = Frozen(Color.FromRgb(0x1E, 0x27, 0x33), GridThickness);
    private static readonly Pen BodyPen = Frozen(Color.FromRgb(0xA7, 0xE8, 0xB8), 1);
    private static readonly Pen HeadPen = Frozen(Color.FromRgb(0xF2, 0xF5, 0xF8), 2);
    private static readonly Pen FoodPen = Frozen(Color.FromRgb(0xFF, 0xE0, 0xD0), 2);
    private static readonly Pen FoodCorePen = Frozen(Color.FromRgb(0x0C, 0x10, 0x16), 2);
    private static readonly Pen DeadlyWallPen = Frozen(Color.FromRgb(0xF2, 0xF5, 0xF8), WallThickness);
    private static readonly Pen WrapWallPen = WrapPen();

    public Game? Game
    {
        get => (Game?)GetValue(GameProperty);
        set => SetValue(GameProperty, value);
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);

        var game = Game;

        if (game is null)
        {
            return;
        }

        var board = game.Board;
        var cellSize = Math.Floor(Math.Min(ActualWidth / board.Width, ActualHeight / board.Height));

        if (cellSize < MinimumCellSize)
        {
            return;
        }

        var field = new Rect(
            Math.Floor((ActualWidth - (cellSize * board.Width)) / 2),
            Math.Floor((ActualHeight - (cellSize * board.Height)) / 2),
            cellSize * board.Width,
            cellSize * board.Height);

        drawingContext.DrawRectangle(FieldBrush, null, field);
        DrawGrid(drawingContext, field, board, cellSize);

        if (game.Food is { } food)
        {
            DrawFood(drawingContext, field, cellSize, food);
        }

        DrawSnake(drawingContext, field, cellSize, game);
        DrawWall(drawingContext, field, DifficultyRules.For(game.Difficulty).WrapsAtWall);
    }

    private static void DrawGrid(DrawingContext drawingContext, Rect field, Board board, double cellSize)
    {
        for (var x = 1; x < board.Width; x++)
        {
            var line = field.X + (x * cellSize);
            drawingContext.DrawLine(GridPen, new Point(line, field.Top), new Point(line, field.Bottom));
        }

        for (var y = 1; y < board.Height; y++)
        {
            var line = field.Y + (y * cellSize);
            drawingContext.DrawLine(GridPen, new Point(field.Left, line), new Point(field.Right, line));
        }
    }

    private static void DrawSnake(DrawingContext drawingContext, Rect field, double cellSize, Game game)
    {
        var cells = game.Snake.Cells;

        for (var i = cells.Count - 1; i > 0; i--)
        {
            var body = CellRect(field, cellSize, cells[i]);
            var corner = cellSize * BodyCornerRatio;
            drawingContext.DrawRoundedRectangle(BodyBrush, BodyPen, body, corner, corner);
        }

        var headRect = CellRect(field, cellSize, game.Snake.Head);
        var headCorner = cellSize * HeadCornerRatio;
        drawingContext.DrawRoundedRectangle(HeadBrush, HeadPen, headRect, headCorner, headCorner);
        DrawEyes(drawingContext, headRect, cellSize, game.Direction);
    }

    private static void DrawEyes(DrawingContext drawingContext, Rect head, double cellSize, Direction direction)
    {
        var center = new Point(head.X + (head.Width / 2), head.Y + (head.Height / 2));
        var forward = UnitOf(direction);
        var side = new Vector(-forward.Y, forward.X);
        var radius = cellSize * EyeRadiusRatio;
        var ahead = forward * (cellSize * EyeForwardRatio);
        var apart = side * (cellSize * EyeSideRatio);

        drawingContext.DrawEllipse(EyeBrush, null, center + ahead + apart, radius, radius);
        drawingContext.DrawEllipse(EyeBrush, null, center + ahead - apart, radius, radius);
    }

    private static void DrawFood(DrawingContext drawingContext, Rect field, double cellSize, Cell food)
    {
        var cell = CellRect(field, cellSize, food);
        var center = new Point(cell.X + (cell.Width / 2), cell.Y + (cell.Height / 2));

        drawingContext.DrawEllipse(FoodBrush, FoodPen, center, cellSize * FoodRadiusRatio, cellSize * FoodRadiusRatio);
        drawingContext.DrawEllipse(
            null, FoodCorePen, center, cellSize * FoodCoreRadiusRatio, cellSize * FoodCoreRadiusRatio);
    }

    private static void DrawWall(DrawingContext drawingContext, Rect field, bool wrapsAtWall)
    {
        var pen = wrapsAtWall ? WrapWallPen : DeadlyWallPen;
        var half = pen.Thickness / 2;

        drawingContext.DrawRectangle(null, pen, Rect.Inflate(field, half, half));
    }

    private static Rect CellRect(Rect field, double cellSize, Cell cell)
    {
        var inset = cellSize * SegmentInset;

        return new Rect(
            field.X + (cell.X * cellSize) + inset,
            field.Y + (cell.Y * cellSize) + inset,
            cellSize - (2 * inset),
            cellSize - (2 * inset));
    }

    private static Vector UnitOf(Direction direction) => direction switch
    {
        Direction.Up => new Vector(0, -1),
        Direction.Down => new Vector(0, 1),
        Direction.Left => new Vector(-1, 0),
        _ => new Vector(1, 0)
    };

    private static Pen WrapPen()
    {
        var pen = new Pen(Frozen(Color.FromRgb(0x7A, 0xC8, 0xFF)), WallThickness)
        {
            DashStyle = new DashStyle([2, 2], 0)
        };

        pen.Freeze();
        return pen;
    }

    private static Brush Frozen(Color color)
    {
        var brush = new SolidColorBrush(color);
        brush.Freeze();
        return brush;
    }

    private static Pen Frozen(Color color, double thickness)
    {
        var pen = new Pen(Frozen(color), thickness);
        pen.Freeze();
        return pen;
    }
}
