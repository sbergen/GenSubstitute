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
        var builder = Gen.Substitute<ITestInterface>().Build();
        builder.Method(default(int)).Returns(42);
        builder.Object.Method(0).Should().Be(42);
    }
    
    [Fact]
    public static void Default_MatchesDefaultValue_WithReferenceType()
    {
        var builder = Gen.Substitute<ITestInterface>().Build();
        builder.Method(null).Returns(42);
        builder.Object.Method(null).Should().Be(42);
    }
}
