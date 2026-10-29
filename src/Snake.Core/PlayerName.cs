namespace Snake.Core;

public sealed record PlayerName
{
    public const int MaxLength = 20;

    private const string DefaultName = "Игрок";

    private PlayerName(string value) => Value = value;

    public static PlayerName Default { get; } = new(DefaultName);

    public string Value { get; }

    public static PlayerName From(string? value)
    {
        var trimmed = (value ?? string.Empty).Trim();

        if (trimmed.Length == 0)
        {
            return Default;
        }

        return new PlayerName(trimmed.Length > MaxLength ? trimmed[..MaxLength] : trimmed);
    }

    public override string ToString() => Value;
}
