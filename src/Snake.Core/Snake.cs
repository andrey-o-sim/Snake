namespace Snake.Core;

public sealed class Snake
{
    public const int InitialLength = 3;
    public const Direction InitialDirection = Direction.Right;

    private readonly Cell[] cells;

    private Snake(Cell[] cells) => this.cells = cells;

    public static Snake AtCenterOf(Board board)
    {
        var head = new Cell(board.Width / 2, board.Height / 2);
        var cells = new Cell[InitialLength];

        for (var i = 0; i < InitialLength; i++)
        {
            cells[i] = head with { X = head.X - i };
        }

        return new Snake(cells);
    }

    public IReadOnlyList<Cell> Cells => cells;

    public Cell Head => cells[0];

    public Cell Tail => cells[^1];

    public int Length => cells.Length;

    public bool Occupies(Cell cell) => Array.IndexOf(cells, cell) >= 0;

    public bool HitsBodyAt(Cell cell, bool tailStays)
    {
        // FR-12: без роста хвост уходит на этом же тике, поэтому его клетка телом уже не считается.
        var bodyLength = tailStays ? cells.Length : cells.Length - 1;

        for (var i = 0; i < bodyLength; i++)
        {
            if (cells[i] == cell)
            {
                return true;
            }
        }

        return false;
    }

    public Snake MoveTo(Cell head, bool grow)
    {
        var moved = new Cell[grow ? cells.Length + 1 : cells.Length];
        moved[0] = head;
        Array.Copy(cells, 0, moved, 1, moved.Length - 1);
        return new Snake(moved);
    }
}
