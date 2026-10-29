namespace Snake.Core.Tests;

internal static class FreeCell
{
    internal static int IndexOf(Cell target, Board board, IEnumerable<Cell> occupied)
    {
        var taken = occupied.ToHashSet();
        var index = 0;

        foreach (var cell in board.Cells)
        {
            if (taken.Contains(cell))
            {
                continue;
            }

            if (cell == target)
            {
                return index;
            }

            index++;
        }

        throw new ArgumentException($"Cell {target} is not free on this board", nameof(target));
    }
}
