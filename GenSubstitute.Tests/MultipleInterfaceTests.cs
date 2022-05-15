using FluentAssertions;
using Xunit;

namespace GenSubstitute.Tests;

public class MultipleInterfaceTests
{
    public interface IBase
    {
        int BaseMethod();
    }
    
    public interface IDerived : IBase
    {
        int DerivedMethod();
    }

    [Fact]
    public void InheritedMethod_IsImplemented()
    {
        var substitute = Gen.Substitute<IDerived>().Create();
        
        substitute.SetUp.BaseMethod().Returns(1);
        substitute.SetUp.DerivedMethod().Returns(2);
        
        substitute.Object.BaseMethod().Should().Be(1);
        substitute.Object.DerivedMethod().Should().Be(2);
    }
}
