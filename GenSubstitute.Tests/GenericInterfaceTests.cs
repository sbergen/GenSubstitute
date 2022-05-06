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
        var builder = Gen.Substitute<IGeneric<int>>().Build();
        builder.MethodReturningT().Returns(42);
        builder.Object.MethodReturningT().Should().Be(42);
    }

    [Fact]
    public static void GenericInterface_CanConfigureArgument()
    {
        var builder = Gen.Substitute<IGeneric<int>>().Build();
        int? receivedValue = null;
        
        builder.MethodTakingT(42).Configure(val => receivedValue = val);
        
        builder.Object.MethodTakingT(42);
        receivedValue.Should().Be(42);
    }
}
