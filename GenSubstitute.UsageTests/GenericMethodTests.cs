using FluentAssertions;
using Xunit;

namespace GenSubstitute.UsageTests;

public static class GenericMethodTests
{
    public interface ITestInterface
    {
        int GenericMethod<T>();
        int GenericMethod<T1, T2>();
    }
    
    [Fact]
    public static void GenericMethods_CanConfiguredSeparately()
    {
        var substitute = Gen.Substitute<ITestInterface>().Create();
        substitute.SetUp.GenericMethod<int>().Returns(1);
        substitute.SetUp.GenericMethod<int, double>().Returns(2);

        substitute.Object.GenericMethod<int>().Should().Be(1, "matching generic argument");
        substitute.Object.GenericMethod<double>().Should().Be(0, "non matching generic argument");
        
        substitute.Object.GenericMethod<int, double>().Should().Be(2, "matching generic arguments");
        substitute.Object.GenericMethod<double, float>().Should().Be(0, "non matching generic arguments");
    }
}
