namespace Snake.Core.Tests;

public class FoodTests
{
    private const int TicksToWatch = 200;

    [Fact]
    public void Food_never_appears_under_the_snake_FR18_FR19()
    {
        var game = Game.Start(Difficulty.Easy, new Random(Scenario.Seed));

        for (var tick = 0; tick < TicksToWatch && game.Status == GameStatus.Running; tick++)
        {
            game = game.Tick();

            Assert.NotNull(game.Food);
            Assert.False(game.Snake.Occupies(game.Food.Value));
        }
    }

    [Fact]
    public void New_food_appears_on_the_same_tick_the_old_one_is_eaten_FR20()
    {
        var game = Scenario.GameWithFoodInFrontOfTheHead(Difficulty.Normal);
        var eaten = game.Food;

        game = game.Tick();

        Assert.NotEqual(eaten, game.Food);
        Assert.NotNull(game.Food);
        Assert.False(game.Snake.Occupies(game.Food.Value));
        Assert.Equal(GameStatus.Running, game.Status);
    }

    [Fact]
    public void Food_stays_where_it_is_while_the_snake_does_not_eat_it_FR18()
    {
        var game = Scenario.StartedGame();
        var food = game.Food;

        game = Scenario.Move(game, Direction.Right, 5);

        Assert.Equal(food, game.Food);
    }

    [Fact]
    public void The_same_seed_gives_the_same_food_NFR03()
    {
        var first = Game.Start(Difficulty.Normal, new Random(Scenario.Seed));
        var second = Game.Start(Difficulty.Normal, new Random(Scenario.Seed));

        Assert.Equal(first.Food, second.Food);

        for (var tick = 0; tick < 20; tick++)
        {
            first = first.Tick();
            second = second.Tick();
            Assert.Equal(first.Food, second.Food);
            Assert.Equal(first.Snake.Cells, second.Snake.Cells);
        }
    }
}
