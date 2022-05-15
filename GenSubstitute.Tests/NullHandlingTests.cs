using FluentAssertions;
using Xunit;

namespace GenSubstitute.Tests;

public static class DefaultHandlingTests
{
    public interface ITestInterface
    {
        int Method(int val);
        int Method(object? val);
    }

    [Fact]
    public static void Default_MatchesDefaultValue_WithValueType()
    {
        var substitute = Gen.Substitute<ITestInterface>().Build();
        substitute.SetUp.Method(default(int)).Returns(42);
        substitute.Object.Method(0).Should().Be(42);
    }
    
    [Fact]
    public static void Default_MatchesDefaultValue_WithReferenceType()
    {
        var substitute = Gen.Substitute<ITestInterface>().Build();
        substitute.SetUp.Method(default(object)).Returns(42);
        substitute.Object.Method(null).Should().Be(42);
    }
    
    [Fact]
    public static void Default_DoesNotMatchNonDefaultValue_WithValueType()
    {
        var substitute = Gen.Substitute<ITestInterface>().Build();
        substitute.SetUp.Method(default(int)).Returns(42);
        substitute.Object.Method(1).Should().Be(0);
    }
    
    [Fact]
    public static void Default_DoesNotMatchNonDefaultValue_WithReferenceType()
    {
        var substitute = Gen.Substitute<ITestInterface>().Build();
        substitute.SetUp.Method(null).Returns(42);
        substitute.Object.Method(new object()).Should().Be(0);
    }
}
