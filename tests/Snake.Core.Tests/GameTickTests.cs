namespace Snake.Core.Tests;

public class GameTickTests
{
    [Fact]
    public void Tick_without_food_moves_the_head_one_cell_and_drops_the_tail_FR09_FR11()
    {
        var game = Scenario.StartedGame();
        var tail = game.Snake.Tail;

        game = game.Tick();

        Assert.Equal(
            new[] { new Cell(11, 10), new Cell(10, 10), new Cell(9, 10) },
            game.Snake.Cells);
        Assert.Equal(Snake.InitialLength, game.Snake.Length);
        Assert.DoesNotContain(tail, game.Snake.Cells);
    }

    [Fact]
    public void Tick_onto_food_makes_the_snake_one_cell_longer_and_keeps_the_tail_FR11()
    {
        var game = Scenario.GameWithFoodInFrontOfTheHead(Difficulty.Normal);
        var tail = game.Snake.Tail;

        game = game.Tick();

        Assert.Equal(Snake.InitialLength + 1, game.Snake.Length);
        Assert.Equal(tail, game.Snake.Tail);
        Assert.Equal(new Cell(11, 10), game.Snake.Head);
    }

    [Fact]
    public void Head_may_enter_the_cell_the_tail_leaves_on_the_same_tick_FR12()
    {
        var board = new Board(4, 4);
        var start = Snake.AtCenterOf(board);
        var afterEating = new[] { new Cell(3, 2), new Cell(2, 2), new Cell(1, 2), new Cell(0, 2) };

        var random = new ScriptedRandom(
            FreeCell.IndexOf(new Cell(3, 2), board, start.Cells),
            FreeCell.IndexOf(new Cell(0, 0), board, afterEating));

        var game = Game.Start(board, Difficulty.Normal, random);

        game = game.Tick();
        Assert.Equal(afterEating, game.Snake.Cells);

        game = game.Turn(Direction.Up).Tick();
        game = game.Turn(Direction.Left).Tick();

        var tail = game.Snake.Tail;
        Assert.Equal(new Cell(2, 2), tail);

        game = game.Turn(Direction.Down).Tick();

        Assert.Equal(GameStatus.Running, game.Status);
        Assert.Equal(tail, game.Snake.Head);
    }
}
