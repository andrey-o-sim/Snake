namespace Snake.Core.Tests;

public class WallTests
{
    [Fact]
    public void Easy_wraps_at_the_right_edge_FR22_FR23()
    {
        var game = Scenario.Move(Scenario.StartedGame(Difficulty.Easy), Direction.Right, 10);

        Assert.Equal(GameStatus.Running, game.Status);
        Assert.Equal(new Cell(0, 10), game.Snake.Head);
    }

    [Fact]
    public void Easy_wraps_at_the_left_edge_FR22_FR23()
    {
        var game = Scenario.Move(Scenario.StartedGame(Difficulty.Easy), Direction.Up, 1);

        game = Scenario.Move(game, Direction.Left, 11);

        Assert.Equal(GameStatus.Running, game.Status);
        Assert.Equal(new Cell(19, 9), game.Snake.Head);
    }

    [Fact]
    public void Easy_wraps_at_the_top_edge_FR22_FR23()
    {
        var game = Scenario.Move(Scenario.StartedGame(Difficulty.Easy), Direction.Up, 11);

        Assert.Equal(GameStatus.Running, game.Status);
        Assert.Equal(new Cell(10, 19), game.Snake.Head);
    }

    [Fact]
    public void Easy_wraps_at_the_bottom_edge_FR22_FR23()
    {
        var game = Scenario.Move(Scenario.StartedGame(Difficulty.Easy), Direction.Down, 10);

        Assert.Equal(GameStatus.Running, game.Status);
        Assert.Equal(new Cell(10, 0), game.Snake.Head);
    }

    [Fact]
    public void Wrapping_keeps_the_direction_FR23()
    {
        var game = Scenario.Move(Scenario.StartedGame(Difficulty.Easy), Direction.Right, 10);

        Assert.Equal(Direction.Right, game.Direction);
    }

    [Fact]
    public void Food_can_be_eaten_on_the_cell_right_after_wrapping_FR24()
    {
        var board = Board.Default;
        var snake = Snake.AtCenterOf(board);
        var random = new ScriptedRandom(FreeCell.IndexOf(new Cell(0, 10), board, snake.Cells), 0);

        var game = Scenario.Move(Game.Start(board, Difficulty.Easy, random), Direction.Right, 10);

        Assert.Equal(new Cell(0, 10), game.Snake.Head);
        Assert.Equal(Snake.InitialLength + 1, game.Snake.Length);
        Assert.Equal(GameStatus.Running, game.Status);
    }

    [Theory]
    [InlineData(Difficulty.Normal)]
    [InlineData(Difficulty.Hard)]
    public void Leaving_the_board_ends_the_game_FR22(Difficulty difficulty)
    {
        var game = Scenario.Move(Scenario.StartedGame(difficulty), Direction.Right, 9);

        Assert.Equal(GameStatus.Running, game.Status);
        Assert.Equal(new Cell(19, 10), game.Snake.Head);

        var cells = game.Snake.Cells;
        game = game.Tick();

        Assert.Equal(GameStatus.GameOver, game.Status);
        Assert.Equal(cells, game.Snake.Cells);
    }
}
