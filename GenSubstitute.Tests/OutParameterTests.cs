using System;
using FluentAssertions;
using Xunit;

namespace GenSubstitute.Tests;

public static class OutParameterTests
{
    public interface IOutParams
    {
        bool TryGet(out int value);
    }

    [Fact]
    public static void OutParameter_CanBeMocked()
    {
        var builder = Gen.Substitute<IOutParams>().Build();

        builder.Configure.TryGet(Arg.Any).Configure(value =>
        {
            value.Value = 42;
            return true;
        });

        builder.Object.TryGet(out var result).Should().Be(true);
        result.Should().Be(42);
    }
    
    [Fact]
    public static void ReceivedRefArg_ShouldNotBeModifiable()
    {
        var builder = Gen.Substitute<IOutParams>().Build();
        builder.Object.TryGet(out _);

        var receivedCall = builder.Received.TryGet(Arg.Any)[0];
        var modifyReceivedValue = () => receivedCall.Arg1.Value = 0;
        modifyReceivedValue.Should().Throw<InvalidOperationException>().WithMessage("*immutable*");
    }
    
    [Fact]
    public static void DefaultOutValue_ShouldNotBeModifiable()
    {
        var modifyAnyArg = () => Out<int>.Default.Value = 10;
        modifyAnyArg.Should().Throw<InvalidOperationException>().WithMessage("*immutable*");
    }
}
