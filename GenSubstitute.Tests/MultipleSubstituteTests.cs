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
        
        substitute1.Configure.SomeMethod(Arg.Any).Returns(1);
        substitute2.Configure.SomeMethod(Arg.Any).Returns(2);

        substitute1.Object.SomeMethod(0).Should().Be(1);
        substitute2.Object.SomeMethod(0).Should().Be(2);

        substitute1.AllReceived.Count().Should().Be(1);
        substitute2.AllReceived.Count().Should().Be(1);

        context.AllReceived.Count.Should().Be(2);
    }
}
