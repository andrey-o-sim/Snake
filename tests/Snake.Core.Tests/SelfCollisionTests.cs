namespace Snake.Core.Tests;

public class SelfCollisionTests
{
    [Theory]
    [InlineData(Difficulty.Easy)]
    [InlineData(Difficulty.Normal)]
    [InlineData(Difficulty.Hard)]
    public void Hitting_the_body_ends_the_game_on_every_difficulty_FR25(Difficulty difficulty)
    {
        var game = SnakeCurledIntoItself(difficulty);
        var cells = game.Snake.Cells;

        game = game.Turn(Direction.Down).Tick();

        Assert.Equal(GameStatus.GameOver, game.Status);
        Assert.Equal(cells, game.Snake.Cells);
    }

    [Fact]
    public void A_cell_of_the_body_that_is_not_the_tail_still_kills_FR25()
    {
        var game = SnakeCurledIntoItself(Difficulty.Normal);

        Assert.True(game.Snake.Occupies(new Cell(4, 3)));
        Assert.NotEqual(new Cell(4, 3), game.Snake.Tail);
    }

    private static Game SnakeCurledIntoItself(Difficulty difficulty)
    {
        var board = new Board(6, 6);
        var start = Snake.AtCenterOf(board);

        var afterFirstFood = new[]
        {
            new Cell(4, 3), new Cell(3, 3), new Cell(2, 3), new Cell(1, 3)
        };

        var afterSecondFood = new[]
        {
            new Cell(5, 3), new Cell(4, 3), new Cell(3, 3), new Cell(2, 3), new Cell(1, 3)
        };

        var random = new ScriptedRandom(
            FreeCell.IndexOf(new Cell(4, 3), board, start.Cells),
            FreeCell.IndexOf(new Cell(5, 3), board, afterFirstFood),
            FreeCell.IndexOf(new Cell(0, 0), board, afterSecondFood));

        var game = Game.Start(board, difficulty, random);

        game = game.Tick();
        game = game.Tick();
        Assert.Equal(afterSecondFood, game.Snake.Cells);

        game = game.Turn(Direction.Up).Tick();
        game = game.Turn(Direction.Left).Tick();

        return game;
    }
}
