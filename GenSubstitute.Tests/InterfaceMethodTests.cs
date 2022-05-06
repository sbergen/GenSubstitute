using Xunit;
using FluentAssertions;

namespace GenSubstitute.Tests;

public interface IFoo
{
    int MethodReturningInt();
    T GenericMethod<T>();
    void MethodTakingInt(int arg);
    double MultipleArgs(int i1, int i2, int i3);
}

public static class InterfaceMethodTests
{
    [Fact]
    public static void MethodReturnValue_CanBeConfigured_WhenNoArguments()
    {
        var builder = Gen.Substitute<IFoo>().Build();
        builder.MethodReturningInt().Returns(42);
        builder.Object.MethodReturningInt().Should().Be(42);
    }
    
    [Fact]
    public static void GenericMethodReturnValue_CanBeConfigured_WhenNoArguments()
    {
        var builder = Gen.Substitute<IFoo>().Build();
        builder.GenericMethod<int>().Returns(42);
        builder.Object.GenericMethod<int>().Should().Be(42);
    }

    [Fact]
    public static void MethodTakingInt_CanBeConfigured_WithAnyArg()
    {
        var builder = Gen.Substitute<IFoo>().Build();
        int? receivedValue = null;
        builder.MethodTakingInt(Arg<int>.Any).Configure(val => receivedValue = val);
        builder.Object.MethodTakingInt(42);
        receivedValue.Should().Be(42);
    }

    [Fact]
    public static void MethodTakingMultipleArgs_CanBeConfigured()
    {
        var builder = Gen.Substitute<IFoo>().Build();
        builder.MultipleArgs(Arg<int>.Any, Arg<int>.Any, Arg<int>.Any)
            .Configure((i1, i2, i3) => i1 + i2 + i3);
        builder.Object.MultipleArgs(10, 30, 2).Should().Be(42.0);
    }
}
