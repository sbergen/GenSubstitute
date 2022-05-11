using System;
using FluentAssertions;
using Xunit;

namespace GenSubstitute.Tests;

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
        var builder = Gen.Substitute<ITestInterface>().Build();
        builder.Configure.MethodWithReturnValue();
        var callIncompleteMethod = () => builder.Object.MethodWithReturnValue();
        
        callIncompleteMethod.Should().Throw<IncompletelyConfiguredCallException>();
    }
    
    [Fact]
    public void UsingPartiallyConfiguredMethod_ThrowsException_WhenMethodHasNoReturnValue()
    {
        var builder = Gen.Substitute<ITestInterface>().Build();
        builder.Configure.MethodWithoutReturnValue();
        var callIncompleteMethod = () => builder.Object.MethodWithoutReturnValue();
        
        callIncompleteMethod.Should().Throw<IncompletelyConfiguredCallException>();
    }
}
