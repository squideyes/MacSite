using MacSite.Core.Common;
using FluentAssertions;

namespace MacSite.UnitTests;

public class SlugTests
{
    [Theory]
    [InlineData("The Magic Flute", "the-magic-flute")]
    [InlineData("Rigoletto Seattle Opera 2004", "rigoletto-seattle-opera-2004")]
    [InlineData("L'ajo nell Imbarazzo", "l-ajo-nell-imbarazzo")]
    [InlineData("Les Fêtes de Thalie", "les-fetes-de-thalie")]
    [InlineData("  Leading & trailing  ", "leading-trailing")]
    [InlineData("Rappaccini's Daughter", "rappaccini-s-daughter")]
    public void From_produces_expected_slug(string input, string expected)
    {
        Slug.From(input).Should().Be(expected);
    }

    [Fact]
    public void From_collapses_multiple_separators()
    {
        Slug.From("A -- B __ C").Should().Be("a-b-c");
    }

    [Fact]
    public void From_returns_empty_for_blank()
    {
        Slug.From("   ").Should().BeEmpty();
    }
}
