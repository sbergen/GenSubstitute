using System;
using FluentAssertions;
using Xunit;

namespace GenSubstitute.Tests;

// TODO: Better exception types here?
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
        builder.MethodWithReturnValue();
        var callIncompleteMethod = () => builder.Object.MethodWithReturnValue();
        
        callIncompleteMethod.Should().Throw<InvalidOperationException>();
    }
    
    [Fact]
    public void UsingPartiallyConfiguredMethod_ThrowsException_WhenMethodHasNoReturnValue()
    {
        var builder = Gen.Substitute<ITestInterface>().Build();
        builder.MethodWithoutReturnValue();
        var callIncompleteMethod = () => builder.Object.MethodWithoutReturnValue();
        
        callIncompleteMethod.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void ConfiguringMethodTwice_ThrowsException_WhenMethodHasReturnValue()
    {
        var builder = Gen.Substitute<ITestInterface>().Build();
        var configure = builder.MethodWithReturnValue();
        configure.Configure(() => 1);
        
        var configureAgain = () => configure.Configure(() => 1);
        var setReturnValue = () => configure.Returns(1);
        
        configureAgain.Should().Throw<InvalidOperationException>();
        setReturnValue.Should().Throw<InvalidOperationException>();
    }
    
    [Fact]
    public void ConfiguringMethodTwice_ThrowsException_WhenMethodHasNoReturnValue()
    {
        var builder = Gen.Substitute<ITestInterface>().Build();
        var configure = builder.MethodWithoutReturnValue();
        configure.Configure(() => { });
        
        var configureAgain = () => configure.Configure(() => { });
        
        configureAgain.Should().Throw<InvalidOperationException>();
    }
}