namespace Snake.Core.Tests;

public class DirectionQueueTests
{
    [Fact]
    public void Turning_back_by_180_degrees_is_ignored_FR14()
    {
        var game = Scenario.StartedGame();

        game = game.Turn(Direction.Left).Tick();

        Assert.Equal(Direction.Right, game.Direction);
        Assert.Equal(new Cell(11, 10), game.Snake.Head);
    }

    [Fact]
    public void Two_presses_on_one_tick_cannot_turn_the_snake_into_itself_FR15_FR16()
    {
        var game = Scenario.StartedGame();

        game = game.Turn(Direction.Up).Turn(Direction.Down);

        game = game.Tick();
        Assert.Equal(Direction.Up, game.Direction);
        Assert.Equal(new Cell(10, 9), game.Snake.Head);

        game = game.Tick();
        Assert.Equal(Direction.Up, game.Direction);
        Assert.Equal(new Cell(10, 8), game.Snake.Head);
    }

    [Fact]
    public void Two_presses_on_one_tick_are_applied_on_two_ticks_FR16()
    {
        var game = Scenario.StartedGame();

        game = game.Turn(Direction.Up).Turn(Direction.Left);

        game = game.Tick();
        Assert.Equal(Direction.Up, game.Direction);
        Assert.Equal(new Cell(10, 9), game.Snake.Head);

        game = game.Tick();
        Assert.Equal(Direction.Left, game.Direction);
        Assert.Equal(new Cell(9, 9), game.Snake.Head);
    }

    [Fact]
    public void A_third_press_over_the_queue_is_ignored_FR16()
    {
        var game = Scenario.StartedGame();

        game = game.Turn(Direction.Up).Turn(Direction.Left).Turn(Direction.Down);

        game = game.Tick();
        game = game.Tick();
        game = game.Tick();

        Assert.Equal(Direction.Left, game.Direction);
        Assert.Equal(new Cell(8, 9), game.Snake.Head);
    }

    [Fact]
    public void Pressing_the_current_direction_again_changes_nothing_FR17()
    {
        var game = Scenario.StartedGame();

        game = game.Turn(Direction.Right).Turn(Direction.Up);

        game = game.Tick();

        Assert.Equal(Direction.Up, game.Direction);
        Assert.Equal(new Cell(10, 9), game.Snake.Head);
    }
}
