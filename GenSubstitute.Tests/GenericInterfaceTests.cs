using FluentAssertions;
using Xunit;

namespace GenSubstitute.Tests;

public static class GenericInterfaceTests
{
    public interface IGeneric<T>
    {
        T MethodReturningT();
        void MethodTakingT(T val);
    }

    [Fact]
    public static void GenericInterface_CanConfigureReturnValue()
    {
        var substitute = Gen.Substitute<IGeneric<int>>().Create();
        substitute.SetUp.MethodReturningT().Returns(42);
        substitute.Object.MethodReturningT().Should().Be(42);
    }

    [Fact]
    public static void GenericInterface_CanConfigureArgument()
    {
        var substitute = Gen.Substitute<IGeneric<int>>().Create();
        int? receivedValue = null;
        
        substitute.SetUp.MethodTakingT(42).As(val => receivedValue = val);
        
        substitute.Object.MethodTakingT(42);
        receivedValue.Should().Be(42);
    }
}
