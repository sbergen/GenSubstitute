using FluentAssertions;
using Xunit;

namespace GenSubstitute.UsageTests;

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
        var substitute = Gen.Substitute<ITestInterface>().Create();
        substitute.SetUp.MethodReturningInt().Returns(42);
        substitute.Object.MethodReturningInt().Should().Be(42);
    }
    
    [Fact]
    public static void GenericMethodReturnValue_CanBeConfigured_WhenNoArguments()
    {
        var substitute = Gen.Substitute<ITestInterface>().Create();
        substitute.SetUp.GenericMethod<int>().Returns(42);
        substitute.Object.GenericMethod<int>().Should().Be(42);
    }

    [Fact]
    public static void MethodTakingInt_CanBeConfigured_WithAnyArg()
    {
        var substitute = Gen.Substitute<ITestInterface>().Create();
        int? receivedValue = null;
        substitute.SetUp.MethodTakingInt(Arg.Any).As(val => receivedValue = val);
        substitute.Object.MethodTakingInt(42);
        receivedValue.Should().Be(42);
    }

    [Fact]
    public static void MethodTakingMultipleArgs_CanBeConfigured()
    {
        var substitute = Gen.Substitute<ITestInterface>().Create();
        substitute.SetUp.MultipleArgs(Arg.Any, Arg.Any, Arg.Any)
            .As((i1, i2, i3) => i1 + i2 + i3);
        substitute.Object.MultipleArgs(10, 30, 2).Should().Be(42.0);
    }

    [Fact]
    public static void MethodWithOverloads_CanBeConfiguredSeparately()
    {
        var substitute = Gen.Substitute<ITestInterface>().Create();
        substitute.SetUp.MethodWithOverload(Arg<double>.Any).Returns(1);
        substitute.SetUp.MethodWithOverload(Arg<int>.Any).Returns(2);

        substitute.Object.MethodWithOverload(0.0).Should().Be(1);
        substitute.Object.MethodWithOverload(0).Should().Be(2);
    }
}
