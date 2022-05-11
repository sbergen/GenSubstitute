using System;
using FluentAssertions;
using Xunit;

namespace GenSubstitute.Tests;

public class RefParametersTest
{
    public interface IRefParams
    {
        void Modify(int val, ref int reference);
    }

    [Fact]
    public void RefParameterMethod_CanBeMockedWithSpecificValue()
    {
        var builder = Gen.Substitute<IRefParams>().Build();
        
        builder.Configure.Modify(Arg.Any, 0)
            .Configure((i, r) => r.Value = i);
        
        int foo = 0;
        builder.Object.Modify(42, ref foo);
        foo.Should().Be(42);
    }
    
    [Fact]
    public void RefParameterMethod_CanBeMockedUsingAnyValue()
    {
        var builder = Gen.Substitute<IRefParams>().Build();
        
        builder.Configure.Modify(Arg.Any, Arg.Any)
            .Configure((i, r) => r.Value = i);
        
        int foo = 10;
        builder.Object.Modify(42, ref foo);
        foo.Should().Be(42);
    }
    
    [Fact]
    public void RefParameterMethod_ShouldNotChangeMatching_WhenValueModified()
    {
        var builder = Gen.Substitute<IRefParams>().Build();

        builder.Configure.Modify(Arg.Any, 5)
            .Configure((_, r) => r.Value = 10 * r);

        var foo = 5;
        builder.Object.Modify(default, ref foo);

        var bar = 5;
        builder.Object.Modify(default, ref bar);
        
        foo.Should().Be(50);
        bar.Should().Be(50);
    }
    
    [Fact]
    public void RefParameterMethod_ShouldNotReceivedCalls_WhenValueModified()
    {
        var builder = Gen.Substitute<IRefParams>().Build();
        
        builder.Configure.Modify(Arg.Any, 5)
            .Configure((_, r) => r.Value = 10 * r);
        
        var foo = 5;
        builder.Object.Modify(default, ref foo);
        
        var bar = 5;
        builder.Object.Modify(default, ref bar);

        builder.Received.Modify(default, 5).Count.Should().Be(2);
    }

    [Fact]
    public void ReceivedRefArg_ShouldNotBeModifiable()
    {
        var builder = Gen.Substitute<IRefParams>().Build();
        int foo = 0;
        builder.Object.Modify(default, ref foo);

        var receivedCall = builder.Received.Modify(Arg.Any, Arg.Any)[0];
        var modifyReceivedValue = () => receivedCall.Arg2.Value = 0;
        modifyReceivedValue.Should().Throw<InvalidOperationException>().WithMessage("*immutable*");
    }
    
    [Fact]
    public void AnyRefArg_ShouldNotBeModifiable()
    {
        var modifyAnyArg = () => RefArg<int>.Any.Value = 10;
        modifyAnyArg.Should().Throw<InvalidOperationException>().WithMessage("*immutable*");
    }
    
    [Fact]
    public void DefaultRefArg_ShouldNotBeModifiable()
    {
        var modifyAnyArg = () => RefArg<int>.Default.Value = 10;
        modifyAnyArg.Should().Throw<InvalidOperationException>().WithMessage("*immutable*");
    }
}
