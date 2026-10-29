namespace Snake.Core.Tests;

public class PlayerNameTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void An_empty_name_becomes_the_default_name_FR44(string? value)
    {
        Assert.Equal(PlayerName.Default, PlayerName.From(value));
    }

    [Fact]
    public void The_default_name_is_the_word_player_in_russian_FR44()
    {
        Assert.Equal("Игрок", PlayerName.Default.Value);
    }

    [Fact]
    public void A_name_longer_than_twenty_characters_is_cut_FR44()
    {
        var name = PlayerName.From(new string('й', 25));

        Assert.Equal(new string('й', PlayerName.MaxLength), name.Value);
    }

    [Fact]
    public void A_name_of_exactly_twenty_characters_is_kept_FR44()
    {
        var value = new string('a', PlayerName.MaxLength);

        Assert.Equal(value, PlayerName.From(value).Value);
    }

    [Fact]
    public void Spaces_around_a_name_are_removed_FR44()
    {
        Assert.Equal("Аня", PlayerName.From("  Аня  ").Value);
    }

    [Fact]
    public void Two_names_with_the_same_text_are_equal_FR44()
    {
        Assert.Equal(PlayerName.From("Аня"), PlayerName.From("Аня"));
    }

    [Fact]
    public void A_name_reads_as_its_text_FR44()
    {
        Assert.Equal("Аня", PlayerName.From("Аня").ToString());
    }
}
