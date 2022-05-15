using FluentAssertions;
using Xunit;

namespace GenSubstitute.UsageTests;

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
        var substitute = Gen.Substitute<ITestInterface>().Create();
        var configure = substitute.SetUp.MethodWithReturnValue();
        
        configure.As(() => 1);
        substitute.Object.MethodWithReturnValue().Should().Be(1);
        
        configure.Returns(2);
        substitute.Object.MethodWithReturnValue().Should().Be(2);
        
        configure.As(() => 3);
        substitute.Object.MethodWithReturnValue().Should().Be(3);

        configure.Returns(4);
        substitute.Object.MethodWithReturnValue().Should().Be(4);
    }
    
    [Fact]
    public void MethodCanBeReconfigured_WhenItHasNoReturnValue()
    {
        var substitute = Gen.Substitute<ITestInterface>().Create();
        var configure = substitute.SetUp.MethodWithoutReturnValue();

        int val = 0;
        
        configure.As(() => val = 1);
        substitute.Object.MethodWithoutReturnValue();
        val.Should().Be(1);
        
        configure.As(() => val = 2);
        substitute.Object.MethodWithoutReturnValue();
        val.Should().Be(2);
    }
}
