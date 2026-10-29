namespace Snake.Core.Tests;

public class ValueObjectTests
{
    [Theory]
    [InlineData(Direction.Up, 5, 4)]
    [InlineData(Direction.Down, 5, 6)]
    [InlineData(Direction.Left, 4, 5)]
    [InlineData(Direction.Right, 6, 5)]
    public void A_step_moves_the_cell_by_one_in_the_given_direction(Direction direction, int x, int y)
    {
        Assert.Equal(new Cell(x, y), new Cell(5, 5).Step(direction));
    }

    [Fact]
    public void A_step_in_an_unknown_direction_is_rejected()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Cell(5, 5).Step((Direction)42));
    }

    [Theory]
    [InlineData(Direction.Up, Direction.Down)]
    [InlineData(Direction.Down, Direction.Up)]
    [InlineData(Direction.Left, Direction.Right)]
    [InlineData(Direction.Right, Direction.Left)]
    public void Every_direction_has_an_opposite_FR14(Direction direction, Direction expected)
    {
        Assert.Equal(expected, direction.Opposite());
    }

    [Fact]
    public void An_unknown_direction_has_no_opposite()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => ((Direction)42).Opposite());
    }

    [Theory]
    [InlineData(0, 0, true)]
    [InlineData(19, 19, true)]
    [InlineData(-1, 0, false)]
    [InlineData(0, -1, false)]
    [InlineData(20, 0, false)]
    [InlineData(0, 20, false)]
    public void The_board_knows_which_cells_are_inside_it_FR01(int x, int y, bool expected)
    {
        Assert.Equal(expected, Board.Default.Contains(new Cell(x, y)));
    }

    [Theory]
    [InlineData(20, 10, 0, 10)]
    [InlineData(-1, 10, 19, 10)]
    [InlineData(10, 20, 10, 0)]
    [InlineData(10, -1, 10, 19)]
    public void Wrapping_mirrors_the_cell_to_the_opposite_edge_FR23(int x, int y, int wrappedX, int wrappedY)
    {
        Assert.Equal(new Cell(wrappedX, wrappedY), Board.Default.Wrap(new Cell(x, y)));
    }

    [Fact]
    public void The_board_lists_every_cell_once_FR01()
    {
        var cells = Board.Default.Cells.ToList();

        Assert.Equal(Board.Default.CellCount, cells.Count);
        Assert.Equal(Board.Default.CellCount, cells.Distinct().Count());
    }

    [Theory]
    [InlineData(Difficulty.Easy, true)]
    [InlineData(Difficulty.Normal, false)]
    [InlineData(Difficulty.Hard, false)]
    public void Only_the_easy_level_lets_the_snake_pass_through_the_wall_FR22(Difficulty difficulty, bool expected)
    {
        Assert.Equal(expected, DifficultyRules.For(difficulty).WrapsAtWall);
    }

    [Theory]
    [InlineData(Difficulty.Easy, 1)]
    [InlineData(Difficulty.Normal, 2)]
    [InlineData(Difficulty.Hard, 3)]
    public void Every_level_has_its_own_score_multiplier_FR32(Difficulty difficulty, int expected)
    {
        Assert.Equal(expected, DifficultyRules.For(difficulty).ScoreMultiplier);
    }

    [Fact]
    public void An_unknown_difficulty_has_no_rules()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => DifficultyRules.For((Difficulty)42));
    }

    [Fact]
    public void The_snake_starts_three_cells_long_and_looks_to_the_right_FR04_FR05()
    {
        var snake = Snake.AtCenterOf(Board.Default);

        Assert.Equal(Snake.InitialLength, snake.Length);
        Assert.Equal(new Cell(10, 10), snake.Head);
        Assert.Equal(new Cell(8, 10), snake.Tail);
        Assert.Equal(Direction.Right, Snake.InitialDirection);
    }
}
