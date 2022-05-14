using System;
using FluentAssertions;
using Xunit;

namespace GenSubstitute.Tests;

public static class ArgMatcherTests
{
    public interface ITestInterface
    {
        int Method(int i);

        int MethodWithOverload(int i);
        int MethodWithOverload(string? s);
    }

    [Fact]
    public static void ValueAsArg_MatchesOnlyThatValue()
    {
        var builder = Gen.Substitute<ITestInterface>().Build();
        builder.Configure.Method(10).Returns(10);
        
        builder.Object.Method(10).Should().Be(10);
        builder.Object.Method(11).Should().Be(0);
    }
    
    [Fact]
    public static void ArgMatcher_ConfiguresReturnValue_WhenItMatches()
    {
        var builder = Gen.Substitute<ITestInterface>().Build();
        builder.Configure.Method(new(i => i > 10)).Returns(10);
        builder.Object.Method(11).Should().Be(10);
    }
    
    [Fact]
    public static void ArgMatcher_DoesNotConfigureReturnValue_WhenItDoesNotMatch()
    {
        var builder = Gen.Substitute<ITestInterface>().Build();
        builder.Configure.Method(new(i => i < 10)).Returns(10);
        builder.Object.Method(11).Should().Be(default);
    }

    [Fact]
    public static void MultipleMatchers_CanBeUsedAtTheSameTime()
    {
        var builder = Gen.Substitute<ITestInterface>().Build();
        builder.Configure.Method(new(i => i < 0)).Returns(-1);
        builder.Configure.Method(new(i => i > 0)).Returns(1);

        builder.Object.Method(-100).Should().Be(-1);
        builder.Object.Method(-0).Should().Be(0);
        builder.Object.Method(100).Should().Be(1);
    }

    [Fact]
    public static void OverlappingMatchers_ThrowException_WhenInvoked()
    {
        var builder = Gen.Substitute<ITestInterface>().Build();
        builder.Configure.Method(new(i => i < 0)).Returns(-1);
        builder.Configure.Method(new(i => i < -10)).Returns(1);

        Action ambiguousInvoke = () => builder.Object.Method(-20);

        ambiguousInvoke.Should()
            .Throw<AmbiguousConfiguredCallMatchException>()
            .WithMessage("*-20*i < *i < -10");
    }

    [Fact]
    public static void MethodWithOverload_CanBeMatchedWithAny()
    {
        var builder = Gen.Substitute<ITestInterface>().Build();

        builder.Configure.MethodWithOverload(Arg<int>.Any).Returns(1);
        builder.Configure.MethodWithOverload(Arg<string?>.Any).Returns(2);

        builder.Object.MethodWithOverload(0).Should().Be(1);
        builder.Object.MethodWithOverload(null).Should().Be(2);
    }
    
    [Fact]
    public static void MethodWithOverload_CanBeMatchedWithLambda()
    {
        var builder = Gen.Substitute<ITestInterface>().Build();

        builder.Configure.MethodWithOverload(Arg.Matches<int>(i => i == 0)).Returns(1);
        builder.Configure.MethodWithOverload(Arg.Matches<string?>(s => s == null)).Returns(2);

        builder.Object.MethodWithOverload(0).Should().Be(1);
        builder.Object.MethodWithOverload(null).Should().Be(2);
    }
    
    [Fact]
    public static void MethodWithOverload_CanBeMatchedWithValue()
    {
        var builder = Gen.Substitute<ITestInterface>().Build();

        builder.Configure.MethodWithOverload(0).Returns(1);
        builder.Configure.MethodWithOverload(Arg.Is<string?>(null)).Returns(2);

        builder.Object.MethodWithOverload(0).Should().Be(1);
        builder.Object.MethodWithOverload(null).Should().Be(2);
    }
}
