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
        var builder = Gen.Substitute<IDerived>().Build();
        
        builder.BaseMethod().Returns(1);
        builder.DerivedMethod().Returns(2);
        
        builder.Object.BaseMethod().Should().Be(1);
        builder.Object.DerivedMethod().Should().Be(2);
    }
}
