namespace Snake.Core.Tests;

internal static class Scenario
{
    internal const int Seed = 20251029;

    internal static Game StartedGame(Difficulty difficulty = Difficulty.Normal) =>
        Game.Start(difficulty, new ScriptedRandom(0));

    internal static Game GameWithFoodInFrontOfTheHead(Difficulty difficulty)
    {
        var board = Board.Default;
        var snake = Snake.AtCenterOf(board);
        var inFront = snake.Head.Step(Snake.InitialDirection);

        return Game.Start(
            board,
            difficulty,
            new ScriptedRandom(FreeCell.IndexOf(inFront, board, snake.Cells), 0));
    }

    internal static Game Move(Game game, Direction direction, int ticks)
    {
        for (var tick = 0; tick < ticks; tick++)
        {
            game = game.Turn(direction).Tick();
        }

        return game;
    }
}
