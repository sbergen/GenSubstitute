using FluentAssertions;
using Xunit;

namespace GenSubstitute.Tests;

public static class ReceivedCallsTest
{
    public interface ITestInterface
    {
        public void Method();
        public void MethodWithArgs(int arg);
    }
    
    [Fact]
    public static void CallCount_MatchesExpected_WhenNoArgs()
    {
        var builder = Gen.Substitute<ITestInterface>().Build();
        builder.Object.Method();
        builder.Object.Method();

        builder.Received.Method().Should().HaveCount(2);
    }
    
    [Fact]
    public static void CallCount_MatchesExpected_WithArgs()
    {
        var builder = Gen.Substitute<ITestInterface>().Build();
        builder.Object.MethodWithArgs(1);
        builder.Object.MethodWithArgs(2);
        builder.Object.MethodWithArgs(3);
        builder.Object.MethodWithArgs(4);

        builder.Received.MethodWithArgs(Arg<int>.When(i => i <= 2)).Should().HaveCount(2);
        builder.Received.MethodWithArgs(Arg.Any).Should().HaveCount(4);
    }
}
