using System.Linq;
using FluentAssertions;
using GenSubstitute.SourceGenerator.Utilities;
using Xunit;

namespace GenSubstitute.Tests;

public static class GenSourceTextTests
{
    [Fact]
    public static void ToString_ProducesExpectedResult_WithSingleLine()
    {
        var text = new GenSourceText();
        text.AddLine(1, "SomeLine");
        text.ToString().Should().Be("\tSomeLine\n");
    }
    
    [Fact]
    public static void ToString_ProducesExpectedResult_WithMultipleLines()
    {
        var text = new GenSourceText();
        text.AddLine(0, "Zero");
        text.AddLine(1, "One");
        text.AddLine(2, "Two");
        text.ToString().Should().Be("Zero\n\tOne\n\t\tTwo\n");
    }

    [Theory]
    [InlineData(0, 0, 0, "")]
    [InlineData(0, 2, 2, "Ze")]
    [InlineData(4, 2, 2, "\n\t")]
    [InlineData(4, 2, 6, "\n\tOne\n")]
    [InlineData(5, 2, 6, "\tOne\n\t")]
    public static void CopyTo_ProducesExpectedResult(
        int sourceIndex,
        int destinationIndex,
        int count,
        string expectedResult)
    {
        var text = new GenSourceText();
        text.AddLine(0, "Zero");
        text.AddLine(1, "One");
        text.AddLine(2, "Two");

        var buffer = new char[destinationIndex + count + 1];
        text.CopyTo(sourceIndex, buffer, destinationIndex, count);

        new string(buffer, destinationIndex, count).Should().Be(expectedResult);

        buffer.Take(destinationIndex).Should().AllBeEquivalentTo('\0');
        buffer.Last().Should().Be('\0');
    }
    
    [Theory]
    [InlineData(0, '\t')]
    [InlineData(1, '1')]
    [InlineData(2, '\n')]
    [InlineData(3, '\t')]
    [InlineData(4, '\t')]
    [InlineData(5, '2')]
    [InlineData(6, '\n')]
    public static void Indexing_ProducesExpectedResult(int index, char expectedChar)
    {
        var text = new GenSourceText();
        text.AddLine(1, "1");
        text.AddLine(2, "2");
        text[index].Should().Be(expectedChar);
    }
}
