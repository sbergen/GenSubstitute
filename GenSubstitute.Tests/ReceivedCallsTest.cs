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
    public static void Times_MatchesExpected_WhenNoArgs()
    {
        var builder = Gen.Substitute<ITestInterface>().Build();
        builder.Object.Method();
        builder.Object.Method();

        builder.Received.Method().Times(2);
    }
    
    [Fact]
    public static void Times_MatchesExpected_WithArgs()
    {
        var builder = Gen.Substitute<ITestInterface>().Build();

        var mock = builder.Object;
        mock.MethodWithArg(1);
        mock.MethodWithArg(2);
        mock.MethodWithArg(3);
        mock.MethodWithArg(4);

        builder.Received.MethodWithArg(new(i => i <= 2)).Times(2);
        builder.Received.MethodWithArg(Arg.Any).Times(4);
        builder.Received.MethodWithArg(Arg.Any)[1].Arg1.Should().Be(2);
    }

    [Fact]
    public static void ReceivedCalls_AreIncludedInExceptionMessage_WhenNotMatching()
    {
        var builder = Gen.Substitute<ITestInterface>().Build();

        var mock = builder.Object;
        mock.MethodWithArg(1);
        mock.MethodWithArg(2);

        var assert = () => builder.Received.MethodWithArg(Arg.Any).Once();
        assert.Should()
            .Throw<ReceivedCallsAssertionException>()
            .WithMessage("*MethodWithArg(1)*MethodWithArg(2)*");
    }
    
    [Fact]
    public static void Once_AllowsEasyAccessToArgs()
    {
        var builder = Gen.Substitute<ITestInterface>().Build();
        
        builder.Object.MethodWithArg(1);

        builder.Received.MethodWithArg(Arg.Any).Once().Arg1.Should().Be(1);
    }

    [Fact]
    public static void AllReceived_ReturnsExpectedData()
    {
        var builder = Gen.Substitute<ITestInterface>().Build();

        builder.Object.Method();
        builder.Object.MethodWithArg(2);

        var allReceived = builder.AllReceived.ToList();
        
        allReceived.Select(call => call.MethodName).Should()
            .Equal(nameof(ITestInterface.Method), nameof(ITestInterface.MethodWithArg));

        allReceived[0].GetArguments().Should().Equal(Array.Empty<object?>());
        allReceived[1].GetArguments().Should().Equal(2);
    }
}
