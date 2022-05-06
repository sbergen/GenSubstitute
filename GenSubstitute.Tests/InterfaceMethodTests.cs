using Xunit;
using FluentAssertions;

namespace GenSubstitute.Tests;

public static class InterfaceMethodTests
{
    public interface ITestInterface
    {
        int MethodReturningInt();
        T GenericMethod<T>();
        void MethodTakingInt(int arg);
        double MultipleArgs(int i1, int i2, int i3);
        int MethodWithOverload(int i);
        int MethodWithOverload(double d);
    }
    
    [Fact]
    public static void MethodReturnValue_CanBeConfigured_WhenNoArguments()
    {
        var builder = Gen.Substitute<ITestInterface>().Build();
        builder.MethodReturningInt().Returns(42);
        builder.Object.MethodReturningInt().Should().Be(42);
    }
    
    [Fact]
    public static void GenericMethodReturnValue_CanBeConfigured_WhenNoArguments()
    {
        var builder = Gen.Substitute<ITestInterface>().Build();
        builder.GenericMethod<int>().Returns(42);
        builder.Object.GenericMethod<int>().Should().Be(42);
    }

    [Fact]
    public static void MethodTakingInt_CanBeConfigured_WithAnyArg()
    {
        var builder = Gen.Substitute<ITestInterface>().Build();
        int? receivedValue = null;
        builder.MethodTakingInt(Arg<int>.Any).Configure(val => receivedValue = val);
        builder.Object.MethodTakingInt(42);
        receivedValue.Should().Be(42);
    }

    [Fact]
    public static void MethodTakingMultipleArgs_CanBeConfigured()
    {
        var builder = Gen.Substitute<ITestInterface>().Build();
        builder.MultipleArgs(Arg<int>.Any, Arg<int>.Any, Arg<int>.Any)
            .Configure((i1, i2, i3) => i1 + i2 + i3);
        builder.Object.MultipleArgs(10, 30, 2).Should().Be(42.0);
    }

    [Fact]
    public static void MethodWithOverloads_CanBeConfiguredSeparately()
    {
        var builder = Gen.Substitute<ITestInterface>().Build();
        builder.MethodWithOverload(Arg<double>.Any).Returns(1);
        builder.MethodWithOverload(Arg<int>.Any).Returns(2);

        builder.Object.MethodWithOverload(0.0).Should().Be(1);
        builder.Object.MethodWithOverload(0).Should().Be(2);
    }
}
