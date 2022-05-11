using Xunit;
using FluentAssertions;

namespace GenSubstitute.Tests;

public class ReconfigureTests
{
    public interface ITestInterface
    {
        int MethodWithReturnValue();
        void MethodWithoutReturnValue();
    }
    
    [Fact]
    public void MethodCanBeReconfigured_WhenItHasReturnValue()
    {
        var builder = Gen.Substitute<ITestInterface>().Build();
        var configure = builder.Configure.MethodWithReturnValue();
        
        configure.Configure(() => 1);
        builder.Object.MethodWithReturnValue().Should().Be(1);
        
        configure.Returns(2);
        builder.Object.MethodWithReturnValue().Should().Be(2);
        
        configure.Configure(() => 3);
        builder.Object.MethodWithReturnValue().Should().Be(3);

        configure.Returns(4);
        builder.Object.MethodWithReturnValue().Should().Be(4);
    }
    
    [Fact]
    public void MethodCanBeReconfigured_WhenItHasNoReturnValue()
    {
        var builder = Gen.Substitute<ITestInterface>().Build();
        var configure = builder.Configure.MethodWithoutReturnValue();

        int val = 0;
        
        configure.Configure(() => val = 1);
        builder.Object.MethodWithoutReturnValue();
        val.Should().Be(1);
        
        configure.Configure(() => val = 2);
        builder.Object.MethodWithoutReturnValue();
        val.Should().Be(2);
    }
}