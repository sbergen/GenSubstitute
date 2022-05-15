using System;
using FluentAssertions;
using Xunit;

namespace GenSubstitute.UsageTests;

public class RefParametersTest
{
    public interface IRefParams
    {
        void Modify(int val, ref int reference);
    }

    [Fact]
    public void RefParameterMethod_CanBeMockedWithSpecificValue()
    {
        var substitute = Gen.Substitute<IRefParams>().Create();
        
        substitute.SetUp.Modify(Arg.Any, new(0))
            .As((i, r) => r.Value = i);
        
        int foo = 0;
        substitute.Object.Modify(42, ref foo);
        foo.Should().Be(42);
    }
    
    [Fact]
    public void RefParameterMethod_CanBeMockedUsingAnyValue()
    {
        var substitute = Gen.Substitute<IRefParams>().Create();
        
        substitute.SetUp.Modify(Arg.Any, Arg.Any)
            .As((i, r) => r.Value = i);
        
        int foo = 10;
        substitute.Object.Modify(42, ref foo);
        foo.Should().Be(42);
    }
    
    [Fact]
    public void RefParameterMethod_CanBeMockedUsingArgMatcher()
    {
        var substitute = Gen.Substitute<IRefParams>().Create();

        int receivedRefArg = 0;
        substitute.SetUp.Modify(Arg.Any, new(i => i < 20))
            .As((_, r) => receivedRefArg = r.Value);
        
        int foo = 10;
        substitute.Object.Modify(42, ref foo);

        // This call should not match
        foo = 20;
        substitute.Object.Modify(42, ref foo);

        receivedRefArg.Should().Be(10);
    }
    
    [Fact]
    public void RefParameterMethod_ShouldNotChangeMatching_WhenValueModified()
    {
        var substitute = Gen.Substitute<IRefParams>().Create();

        substitute.SetUp.Modify(Arg.Any, new(5))
            .As((_, r) => r.Value = 10 * r);

        var foo = 5;
        substitute.Object.Modify(default, ref foo);

        var bar = 5;
        substitute.Object.Modify(default, ref bar);
        
        foo.Should().Be(50);
        bar.Should().Be(50);
    }
    
    [Fact]
    public void RefParameterMethod_ShouldRetainReceivedCalls_WhenValueModified()
    {
        var substitute = Gen.Substitute<IRefParams>().Create();
        
        substitute.SetUp.Modify(Arg.Any, new(5))
            .As((_, r) => r.Value = 10 * r);
        
        var foo = 5;
        substitute.Object.Modify(default, ref foo);
        
        var bar = 5;
        substitute.Object.Modify(default, ref bar);

        substitute.Received.Modify(default, new(5)).Times(2);
    }

    [Fact]
    public void ReceivedRefArg_ShouldNotBeModifiable()
    {
        var substitute = Gen.Substitute<IRefParams>().Create();
        int foo = 0;
        substitute.Object.Modify(default, ref foo);

        var receivedCall = substitute.Received.Modify(Arg.Any, Arg.Any)[0];
        var modifyReceivedValue = () => receivedCall.Arg2.Value = 0;
        modifyReceivedValue.Should().Throw<InvalidOperationException>().WithMessage("*immutable*");
    }
}
