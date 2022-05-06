using FluentAssertions;
using Xunit;

namespace GenSubstitute.Tests;

public static class ArgMatcherTests
{
    public interface ITestInterface
    {
        int Method(int i);
    }

    [Fact]
    public static void ArgMatcher_ConfiguresReturnValue_WhenItMatches()
    {
        var builder = Gen.Substitute<ITestInterface>().Build();
        builder.Method(Arg<int>.When(i => i > 10)).Returns(10);
        builder.Object.Method(11).Should().Be(10);
    }
    
    [Fact]
    public static void ArgMatcher_DoesNotConfigureReturnValue_WhenItDoesNotMatch()
    {
        var builder = Gen.Substitute<ITestInterface>().Build();
        builder.Method(Arg<int>.When(i => i < 10)).Returns(10);
        builder.Object.Method(11).Should().Be(default);
    }
}
