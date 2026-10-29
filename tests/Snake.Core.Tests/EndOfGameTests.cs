namespace Snake.Core.Tests;

public class EndOfGameTests
{
    [Fact]
    public void Filling_the_whole_board_wins_the_game_FR28()
    {
        var board = new Board(4, 1);
        var game = Game.Start(board, Difficulty.Easy, new ScriptedRandom(0));

        Assert.Equal(new Cell(3, 0), game.Food);

        game = game.Tick();

        Assert.Equal(GameStatus.Won, game.Status);
        Assert.Equal(board.CellCount, game.Snake.Length);
        Assert.Null(game.Food);
        Assert.Equal(Game.PointsPerFood, game.Score);
    }

    [Fact]
    public void A_tick_after_the_game_is_over_changes_nothing_FR26()
    {
        var game = Scenario.Move(Scenario.StartedGame(), Direction.Right, 10);
        Assert.Equal(GameStatus.GameOver, game.Status);

        var cells = game.Snake.Cells;
        var score = game.Score;
        var food = game.Food;

        game = game.Tick();

        Assert.Equal(GameStatus.GameOver, game.Status);
        Assert.Equal(cells, game.Snake.Cells);
        Assert.Equal(score, game.Score);
        Assert.Equal(food, game.Food);
    }

    [Fact]
    public void A_tick_after_the_game_is_won_changes_nothing_FR26()
    {
        var game = Game.Start(new Board(4, 1), Difficulty.Easy, new ScriptedRandom(0)).Tick();
        Assert.Equal(GameStatus.Won, game.Status);

        var cells = game.Snake.Cells;

        game = game.Tick();

        Assert.Equal(GameStatus.Won, game.Status);
        Assert.Equal(cells, game.Snake.Cells);
    }

    [Fact]
    public void A_key_press_after_the_game_is_over_is_ignored_FR26()
    {
        var game = Scenario.Move(Scenario.StartedGame(), Direction.Right, 10);
        Assert.Equal(GameStatus.GameOver, game.Status);

        var head = game.Snake.Head;

        game = game.Turn(Direction.Up).Tick();

        Assert.Equal(Direction.Right, game.Direction);
        Assert.Equal(head, game.Snake.Head);
    }
}
