namespace Snake.Core.Tests;

public class ScoreTests
{
    [Theory]
    [InlineData(Difficulty.Easy, 10)]
    [InlineData(Difficulty.Normal, 20)]
    [InlineData(Difficulty.Hard, 30)]
    public void Eating_food_scores_ten_points_times_the_multiplier_FR29(Difficulty difficulty, int expectedScore)
    {
        var game = Scenario.GameWithFoodInFrontOfTheHead(difficulty);

        game = game.Tick();

        Assert.Equal(expectedScore, game.Score);
    }

    [Fact]
    public void A_tick_without_food_does_not_change_the_score_FR29()
    {
        var game = Scenario.StartedGame();

        game = Scenario.Move(game, Direction.Right, 5);

        Assert.Equal(0, game.Score);
    }

    [Theory]
    [InlineData(Difficulty.Easy, 200)]
    [InlineData(Difficulty.Normal, 130)]
    [InlineData(Difficulty.Hard, 90)]
    public void Tick_interval_follows_the_difficulty_FR32(Difficulty difficulty, int expectedMilliseconds)
    {
        var game = Scenario.StartedGame(difficulty);

        Assert.Equal(TimeSpan.FromMilliseconds(expectedMilliseconds), game.TickInterval);
    }

    [Theory]
    [InlineData(Difficulty.Easy)]
    [InlineData(Difficulty.Normal)]
    [InlineData(Difficulty.Hard)]
    public void Tick_interval_does_not_change_inside_one_game_FR30(Difficulty difficulty)
    {
        var game = Scenario.GameWithFoodInFrontOfTheHead(difficulty);
        var interval = game.TickInterval;

        game = Scenario.Move(game, Direction.Right, 5);

        Assert.True(game.Score > 0);
        Assert.Equal(interval, game.TickInterval);
    }
}
