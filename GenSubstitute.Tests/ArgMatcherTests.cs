using System;
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
    public static void ValueAsArg_MatchesOnlyThatValue()
    {
        var builder = Gen.Substitute<ITestInterface>().Build();
        builder.Method(10).Returns(10);
        
        builder.Object.Method(10).Should().Be(10);
        builder.Object.Method(11).Should().Be(0);
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

    [Fact]
    public static void MultipleMatchers_CanBeUsedAtTheSameTime()
    {
        var builder = Gen.Substitute<ITestInterface>().Build();
        builder.Method(Arg<int>.When(i => i < 0)).Returns(-1);
        builder.Method(Arg<int>.When(i => i > 0)).Returns(1);

        builder.Object.Method(-100).Should().Be(-1);
        builder.Object.Method(-0).Should().Be(0);
        builder.Object.Method(100).Should().Be(1);
    }

    [Fact]
    public static void OverlappingMatchers_ThrowException_WhenInvoked()
    {
        var builder = Gen.Substitute<ITestInterface>().Build();
        builder.Method(Arg<int>.When(i => i < 0)).Returns(-1);
        builder.Method(Arg<int>.When(i => i < -10)).Returns(1);

        Action ambiguousInvoke = () => builder.Object.Method(-20);

        // TODO better type here
        ambiguousInvoke.Should().Throw<Exception>();
    }
}
