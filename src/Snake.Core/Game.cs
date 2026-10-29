namespace Snake.Core;

public sealed class Game
{
    public const int PointsPerFood = 10;
    public const int MaxPendingDirections = 2;

    private static readonly IReadOnlyList<Direction> NoPendingDirections = [];

    private readonly DifficultyRules rules;
    private readonly IReadOnlyList<Direction> pendingDirections;
    private readonly Random random;

    private Game(
        Board board,
        Difficulty difficulty,
        DifficultyRules rules,
        Snake snake,
        Cell? food,
        Direction direction,
        IReadOnlyList<Direction> pendingDirections,
        int score,
        GameStatus status,
        Random random)
    {
        Board = board;
        Difficulty = difficulty;
        Snake = snake;
        Food = food;
        Direction = direction;
        Score = score;
        Status = status;
        this.rules = rules;
        this.pendingDirections = pendingDirections;
        this.random = random;
    }

    public static Game Start(Difficulty difficulty, Random random) =>
        Start(Board.Default, difficulty, random);

    public static Game Start(Board board, Difficulty difficulty, Random random)
    {
        var snake = Snake.AtCenterOf(board);

        return new Game(
            board,
            difficulty,
            DifficultyRules.For(difficulty),
            snake,
            PickFood(board, snake, random),
            Snake.InitialDirection,
            NoPendingDirections,
            score: 0,
            GameStatus.Running,
            random);
    }

    public Board Board { get; }

    public Difficulty Difficulty { get; }

    public Snake Snake { get; }

    public Cell? Food { get; }

    public Direction Direction { get; }

    public int Score { get; }

    public GameStatus Status { get; }

    public TimeSpan TickInterval => rules.TickInterval;

    public Game Turn(Direction direction)
    {
        if (Status != GameStatus.Running || pendingDirections.Count >= MaxPendingDirections)
        {
            return this;
        }

        // FR-15: сравниваем с последним направлением в очереди, иначе два нажатия за тик развернут змейку в себя.
        var previous = pendingDirections.Count > 0 ? pendingDirections[^1] : Direction;

        if (direction == previous || direction == previous.Opposite())
        {
            return this;
        }

        return With(Snake, Food, Direction, [.. pendingDirections, direction], Score, Status);
    }

    public Game Tick()
    {
        if (Status != GameStatus.Running)
        {
            return this;
        }

        var direction = NextDirection(out var remaining);
        var target = Snake.Head.Step(direction);

        if (!Board.Contains(target))
        {
            if (!rules.WrapsAtWall)
            {
                return With(Snake, Food, direction, remaining, Score, GameStatus.GameOver);
            }

            target = Board.Wrap(target);
        }

        var eatsFood = Food == target;

        if (Snake.HitsBodyAt(target, eatsFood))
        {
            return With(Snake, Food, direction, remaining, Score, GameStatus.GameOver);
        }

        var snake = Snake.MoveTo(target, eatsFood);

        if (!eatsFood)
        {
            return With(snake, Food, direction, remaining, Score, GameStatus.Running);
        }

        var food = PickFood(Board, snake, random);
        var score = Score + PointsPerFood * rules.ScoreMultiplier;
        var status = food is null ? GameStatus.Won : GameStatus.Running;

        return With(snake, food, direction, remaining, score, status);
    }

    private Direction NextDirection(out IReadOnlyList<Direction> remaining)
    {
        if (pendingDirections.Count == 0)
        {
            remaining = pendingDirections;
            return Direction;
        }

        remaining = [.. pendingDirections.Skip(1)];
        return pendingDirections[0];
    }

    private Game With(
        Snake snake,
        Cell? food,
        Direction direction,
        IReadOnlyList<Direction> pending,
        int score,
        GameStatus status) =>
        new(Board, Difficulty, rules, snake, food, direction, pending, score, status, random);

    private static Cell? PickFood(Board board, Snake snake, Random random)
    {
        var freeCells = FreeCells(board, snake);
        return freeCells.Count == 0 ? null : freeCells[random.Next(freeCells.Count)];
    }

    private static List<Cell> FreeCells(Board board, Snake snake)
    {
        var freeCells = new List<Cell>(board.CellCount - snake.Length);

        foreach (var cell in board.Cells)
        {
            if (!snake.Occupies(cell))
            {
                freeCells.Add(cell);
            }
        }

        return freeCells;
    }
}
