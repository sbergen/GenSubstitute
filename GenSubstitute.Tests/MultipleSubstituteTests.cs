using System.Linq;
using FluentAssertions;
using Xunit;

namespace GenSubstitute.Tests;

public static class MultipleSubstituteTests
{
    public interface ITestInterface
    {
        public int SomeMethod(int i);
    }

    [Fact]
    public static void MultipleSubstitutes_CanBeUsedWithSameContext()
    {
        var context = new SubstitutionContext();
        var substitute1 = Gen.Substitute<ITestInterface>().Build(context);
        var substitute2 = Gen.Substitute<ITestInterface>().Build(context);
        
        substitute1.SetUp.SomeMethod(Arg.Any).Returns(1);
        substitute2.SetUp.SomeMethod(Arg.Any).Returns(2);

        substitute1.Object.SomeMethod(0).Should().Be(1);
        substitute2.Object.SomeMethod(0).Should().Be(2);

        substitute1.AllReceived.Count().Should().Be(1);
        substitute2.AllReceived.Count().Should().Be(1);

        context.Received.Count.Should().Be(2);
    }

    [Fact]
    public static void InOrder_PassesAsExpected_WithMultipleMocks()
    {
        var context = new SubstitutionContext();
        var substitute1 = Gen.Substitute<ITestInterface>().Build(context, "sub1");
        var substitute2 = Gen.Substitute<ITestInterface>().Build(context, "sub2");

        substitute1.Object.SomeMethod(1);
        substitute2.Object.SomeMethod(2);

        context.Received.InOrder(
            substitute1.Match.SomeMethod(1),
            substitute2.Match.SomeMethod(2));
    }
    
    [Fact]
    public static void InOrder_FailsAsExpected_WhenSubstitutesDoNotMatch()
    {
        var context = new SubstitutionContext();
        var substitute1 = Gen.Substitute<ITestInterface>().Build(context, "sub1");
        var substitute2 = Gen.Substitute<ITestInterface>().Build(context, "sub2");

        substitute1.Object.SomeMethod(1);
        substitute2.Object.SomeMethod(2);

        var assert = () => context.Received.InOrder(
            substitute1.Match.SomeMethod(1),
            substitute1.Match.SomeMethod(2));
        assert.Should().Throw<ReceivedCallsAssertionException>();
    }
}
