using FluentAssertions;
using Xunit;

namespace GenSubstitute.Tests;

public static class ArgTests
{
    [Fact]
    public static void ToString_ReturnsLambdaExpression()
    {
        new Arg<int>(i => i > 0).ToString().Should().Be("i => i > 0");
    }
    
    [Fact]
    public static void ToString_ReturnsValueExpression()
    {
        new Arg<int>(1 + 2).ToString().Should().Be("1 + 2");
    }

    [Fact]
    public static void ToString_ReturnsValue_WhenImplicitlyConverted()
    {
        // This is not optimal, but AFAIK can't be fixed.
        ((Arg<int>)(2 + 2)).ToString().Should().Be("4");
    }

    [Fact]
    public static void ToString_ReturnsNullInfo_WhenExpressionAttributeNotAvailable()
    {
        // ReSharper disable once RedundantArgumentDefaultValue
        new Arg<string?>((string?)null, null).ToString().Should().Be($"({typeof(string)})null");
    }
    
    [Fact]
    public static void ToString_ExpressionInfo_WhenExpressionAttributeNotAvailable()
    {
        // ReSharper disable once RedundantArgumentDefaultValue
        new Arg<string>(s => s.Length == 2, null).ToString().Should().Be($"expression on {typeof(string)}");
    }
}
