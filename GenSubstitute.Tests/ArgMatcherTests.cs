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
        var substitute = Gen.Substitute<ITestInterface>().Build();
        substitute.SetUp.Method(10).Returns(10);
        
        substitute.Object.Method(10).Should().Be(10);
        substitute.Object.Method(11).Should().Be(0);
    }
    
    [Fact]
    public static void ArgMatcher_ConfiguresReturnValue_WhenItMatches()
    {
        var substitute = Gen.Substitute<ITestInterface>().Build();
        substitute.SetUp.Method(new(i => i > 10)).Returns(10);
        substitute.Object.Method(11).Should().Be(10);
    }
    
    [Fact]
    public static void ArgMatcher_DoesNotConfigureReturnValue_WhenItDoesNotMatch()
    {
        var substitute = Gen.Substitute<ITestInterface>().Build();
        substitute.SetUp.Method(new(i => i < 10)).Returns(10);
        substitute.Object.Method(11).Should().Be(default);
    }

    [Fact]
    public static void MultipleMatchers_CanBeUsedAtTheSameTime()
    {
        var substitute = Gen.Substitute<ITestInterface>().Build();
        substitute.SetUp.Method(new(i => i < 0)).Returns(-1);
        substitute.SetUp.Method(new(i => i > 0)).Returns(1);

        substitute.Object.Method(-100).Should().Be(-1);
        substitute.Object.Method(-0).Should().Be(0);
        substitute.Object.Method(100).Should().Be(1);
    }

    [Fact]
    public static void OverlappingMatchers_ThrowException_WhenInvoked()
    {
        var substitute = Gen.Substitute<ITestInterface>().Build();
        substitute.SetUp.Method(new(i => i < 0)).Returns(-1);
        substitute.SetUp.Method(new(i => i < -10)).Returns(1);

        Action ambiguousInvoke = () => substitute.Object.Method(-20);

        ambiguousInvoke.Should()
            .Throw<AmbiguousConfiguredCallMatchException>()
            .WithMessage("*-20*i < 0*i < -10*");
    }

    [Fact]
    public static void MethodWithOverload_CanBeMatchedWithAny()
    {
        var substitute = Gen.Substitute<ITestInterface>().Build();

        substitute.SetUp.MethodWithOverload(Arg<int>.Any).Returns(1);
        substitute.SetUp.MethodWithOverload(Arg<string?>.Any).Returns(2);

        substitute.Object.MethodWithOverload(0).Should().Be(1);
        substitute.Object.MethodWithOverload(null).Should().Be(2);
    }
    
    [Fact]
    public static void MethodWithOverload_CanBeMatchedWithLambda()
    {
        var substitute = Gen.Substitute<ITestInterface>().Build();

        substitute.SetUp.MethodWithOverload(Arg.Matches<int>(i => i == 0)).Returns(1);
        substitute.SetUp.MethodWithOverload(Arg.Matches<string?>(s => s == null)).Returns(2);

        substitute.Object.MethodWithOverload(0).Should().Be(1);
        substitute.Object.MethodWithOverload(null).Should().Be(2);
    }
    
    [Fact]
    public static void MethodWithOverload_CanBeMatchedWithValue()
    {
        var substitute = Gen.Substitute<ITestInterface>().Build();

        substitute.SetUp.MethodWithOverload(0).Returns(1);
        substitute.SetUp.MethodWithOverload(Arg.Is<string?>(null)).Returns(2);

        substitute.Object.MethodWithOverload(0).Should().Be(1);
        substitute.Object.MethodWithOverload(null).Should().Be(2);
    }
}
