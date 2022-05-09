using FluentAssertions;
using Xunit;

namespace GenSubstitute.Tests;

public class GenericMethodTests
{
    public interface ITestInterface
    {
        int GenericMethod<T>();
        int GenericMethod<T1, T2>();
    }
    
    [Fact]
    public static void GenericMethods_CanConfiguredSeparately()
    {
        var builder = Gen.Substitute<ITestInterface>().Build();
        builder.Configure.GenericMethod<int>().Returns(1);
        builder.Configure.GenericMethod<int, double>().Returns(2);

        builder.Object.GenericMethod<int>().Should().Be(1, "matching generic argument");
        builder.Object.GenericMethod<double>().Should().Be(0, "non matching generic argument");
        
        builder.Object.GenericMethod<int, double>().Should().Be(2, "matching generic arguments");
        builder.Object.GenericMethod<double, float>().Should().Be(0, "non matching generic arguments");
    }
}
