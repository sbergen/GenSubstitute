using FluentAssertions;
using Xunit;

namespace GenSubstitute.UsageTests;

public class MisuseTests
{
    public interface ITestInterface
    {
        int MethodWithReturnValue();
        void MethodWithoutReturnValue();
    }
    
    [Fact]
    public void UsingPartiallyConfiguredMethod_ThrowsException_WhenMethodHasReturnValue()
    {
        var substitute = Gen.Substitute<ITestInterface>().Create();
        substitute.SetUp.MethodWithReturnValue();
        var callIncompleteMethod = () => substitute.Object.MethodWithReturnValue();
        
        callIncompleteMethod.Should().Throw<IncompletelyConfiguredCallException>();
    }
    
    [Fact]
    public void UsingPartiallyConfiguredMethod_ThrowsException_WhenMethodHasNoReturnValue()
    {
        var substitute = Gen.Substitute<ITestInterface>().Create();
        substitute.SetUp.MethodWithoutReturnValue();
        var callIncompleteMethod = () => substitute.Object.MethodWithoutReturnValue();
        
        callIncompleteMethod.Should().Throw<IncompletelyConfiguredCallException>();
    }
}
