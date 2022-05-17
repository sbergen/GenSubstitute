using System;
using FluentAssertions;
using Xunit;

namespace GenSubstitute.UsageTests;

public class DefaultArgumentTests
{
    public interface IWithDefaultArguments
    {
        int MethodWithVariousArgs(UriKind k = UriKind.Absolute, string foo = "foo", double d = 42.0);
    }

    [Fact]
    public void MethodWithDefaultArgument_ShouldBeConfigurableWithoutSpecifyingIt()
    {
        var substitute = Gen.Substitute<IWithDefaultArguments>().Create();
        substitute.SetUp.MethodWithVariousArgs().Returns(1);
        substitute.Object.MethodWithVariousArgs().Should().Be(1);
    }
}
