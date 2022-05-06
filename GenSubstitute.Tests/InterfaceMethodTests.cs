using Xunit;
using FluentAssertions;

namespace GenSubstitute.Tests;

public interface IFoo
{
    int MethodReturningInt();
    T GenericMethod<T>();
}

public class InterfaceMethodTests
{
    
    [Fact]
    public static void MethodReturnValue_CanBeConfigured_WhenNoArguments()
    {
        var mock = Gen.Substitute<IFoo>().Configure();
        mock.MethodReturningInt().Returns(() => 42);
        mock.Object.MethodReturningInt().Should().Be(42);
    }
    
    [Fact]
    public static void GenericMethod_CanBeConfigured_WhenNoArguments()
    {
        var mock = Gen.Substitute<IFoo>().Configure();
        mock.GenericMethod<int>().Returns(() => 42);
        mock.Object.GenericMethod<int>().Should().Be(42);
    }
}
