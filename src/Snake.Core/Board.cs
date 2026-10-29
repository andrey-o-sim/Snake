namespace Snake.Core;

public readonly record struct Board(int Width, int Height)
{
    public const int DefaultSize = 20;

    public static Board Default { get; } = new(DefaultSize, DefaultSize);

    public int CellCount => Width * Height;

    public IEnumerable<Cell> Cells
    {
        get
        {
            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
                {
                    yield return new Cell(x, y);
                }
            }
        }
    }

    public bool Contains(Cell cell) =>
        cell.X >= 0 && cell.X < Width && cell.Y >= 0 && cell.Y < Height;

    public Cell Wrap(Cell cell) =>
        new(WrapCoordinate(cell.X, Width), WrapCoordinate(cell.Y, Height));

    private static int WrapCoordinate(int coordinate, int size) => ((coordinate % size) + size) % size;
}
