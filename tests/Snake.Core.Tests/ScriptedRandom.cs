namespace Snake.Core.Tests;

internal sealed class ScriptedRandom(params int[] indices) : Random
{
    private int calls;

    public override int Next(int maxValue) => indices[Math.Min(calls++, indices.Length - 1)];
}
