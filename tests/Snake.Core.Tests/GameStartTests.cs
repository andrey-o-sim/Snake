namespace Snake.Core.Tests;

public class GameStartTests
{
    [Fact]
    public void Board_is_twenty_by_twenty_cells_FR01()
    {
        Assert.Equal(20, Board.Default.Width);
        Assert.Equal(20, Board.Default.Height);
        Assert.Equal(400, Board.Default.CellCount);
    }

    [Fact]
    public void Board_does_not_change_while_the_game_runs_FR02()
    {
        var game = Scenario.StartedGame();
        var board = game.Board;

        game = Scenario.Move(game, Direction.Right, 5);

        Assert.Equal(board, game.Board);
    }

    [Fact]
    public void Start_puts_a_snake_of_three_cells_in_the_center_FR04()
    {
        var game = Scenario.StartedGame();

        Assert.Equal(
            new[] { new Cell(10, 10), new Cell(9, 10), new Cell(8, 10) },
            game.Snake.Cells);
    }

    [Fact]
    public void Start_points_the_snake_to_the_right_FR05()
    {
        Assert.Equal(Direction.Right, Scenario.StartedGame().Direction);
    }

    [Fact]
    public void Start_puts_one_food_on_a_cell_the_snake_does_not_occupy_FR06()
    {
        var game = Scenario.StartedGame();

        Assert.NotNull(game.Food);
        Assert.False(game.Snake.Occupies(game.Food.Value));
    }

    [Fact]
    public void Start_sets_the_score_to_zero_FR07()
    {
        Assert.Equal(0, Scenario.StartedGame().Score);
    }

    [Fact]
    public void Start_begins_running_without_any_extra_input_FR08()
    {
        Assert.Equal(GameStatus.Running, Scenario.StartedGame().Status);
    }

    [Fact]
    public void Difficulty_stays_the_same_while_the_game_runs_FR33()
    {
        var game = Scenario.StartedGame(Difficulty.Hard);

        game = Scenario.Move(game, Direction.Right, 5);

        Assert.Equal(Difficulty.Hard, game.Difficulty);
    }
}
