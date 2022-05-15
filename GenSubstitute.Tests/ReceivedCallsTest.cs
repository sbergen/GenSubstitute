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
        var substitute = Gen.Substitute<ITestInterface>().Build();
        substitute.Object.Method();
        substitute.Object.Method();

        substitute.Received.Method().Times(2);
    }
    
    [Fact]
    public static void Times_MatchesExpected_WithArgs()
    {
        var substitute = Gen.Substitute<ITestInterface>().Build();

        var mock = substitute.Object;
        mock.MethodWithArg(1);
        mock.MethodWithArg(2);
        mock.MethodWithArg(3);
        mock.MethodWithArg(4);

        substitute.Received.MethodWithArg(new(i => i <= 2)).Times(2);
        substitute.Received.MethodWithArg(Arg.Any).Times(4);
        substitute.Received.MethodWithArg(Arg.Any)[1].Arg1.Should().Be(2);
    }

    [Fact]
    public static void ReceivedCalls_AreIncludedInExceptionMessage_WhenNotMatching()
    {
        var substitute = Gen.Substitute<ITestInterface>().Build();

        var mock = substitute.Object;
        mock.MethodWithArg(1);
        mock.MethodWithArg(2);

        var assert = () => substitute.Received.MethodWithArg(Arg.Any).Once();
        assert.Should()
            .Throw<ReceivedCallsAssertionException>()
            .WithMessage("*MethodWithArg(1)*MethodWithArg(2)*");
    }
    
    [Fact]
    public static void Once_AllowsEasyAccessToArgs()
    {
        var substitute = Gen.Substitute<ITestInterface>().Build();
        
        substitute.Object.MethodWithArg(1);

        substitute.Received.MethodWithArg(Arg.Any).Once().Arg1.Should().Be(1);
    }

    [Fact]
    public static void AllReceived_ReturnsExpectedData()
    {
        var substitute = Gen.Substitute<ITestInterface>().Build();

        substitute.Object.Method();
        substitute.Object.MethodWithArg(2);

        var allReceived = substitute.AllReceived.ToList();
        
        allReceived.Select(call => call.MethodName).Should()
            .Equal(nameof(ITestInterface.Method), nameof(ITestInterface.MethodWithArg));

        allReceived[0].GetArguments().Should().Equal(Array.Empty<object?>());
        allReceived[1].GetArguments().Should().Equal(2);
    }
}
