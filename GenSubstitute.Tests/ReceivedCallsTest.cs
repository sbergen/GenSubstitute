using System;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace GenSubstitute.Tests;

public static class ReceivedCallsTest
{
    public interface ITestInterface
    {
        public void Method();
        public void MethodWithArg(int arg);
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

        var mock = builder.Object;
        mock.MethodWithArg(1);
        mock.MethodWithArg(2);
        mock.MethodWithArg(3);
        mock.MethodWithArg(4);

        builder.Received.MethodWithArg(Arg<int>.When(i => i <= 2)).Should().HaveCount(2);
        builder.Received.MethodWithArg(Arg.Any).Should().HaveCount(4);
        builder.Received.MethodWithArg(Arg.Any)[1].Arg1.Should().Be(2);
    }
    
    [Fact]
    public static void CallArgs_CanBeAccessedEasily()
    {
        var builder = Gen.Substitute<ITestInterface>().Build();
        
        builder.Object.MethodWithArg(1);
        builder.Object.MethodWithArg(2);
        
        builder.Received.MethodWithArg(Arg.Any)[1].Arg1.Should().Be(2);
    }

    [Fact]
    public static void AllReceived_ReturnsExpectedData()
    {
        var builder = Gen.Substitute<ITestInterface>().Build();

        builder.Object.Method();
        builder.Object.MethodWithArg(2);

        builder.AllReceived.Select(call => call.MethodName).Should()
            .Equal(nameof(ITestInterface.Method), nameof(ITestInterface.MethodWithArg));

        builder.AllReceived[0].GetArguments().Should().Equal(Array.Empty<object?>());
        builder.AllReceived[1].GetArguments().Should().Equal(2);
    }
}
