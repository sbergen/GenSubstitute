using FluentAssertions;
using Microsoft.CodeAnalysis;
using Xunit;

namespace GenSubstitute.Tests;

public class DefaultArgumentTests
{
    public interface IWithDefaultArguments
    {
        int MethodWithVariousArgs(TypeKind k = TypeKind.Array, string foo = "foo", double d = 42.0);
    }

    [Fact]
    public void MethodWithDefaultArgument_ShouldBeConfigurableWithoutSpecifyingIt()
    {
        var substitute = Gen.Substitute<IWithDefaultArguments>().Create();
        substitute.SetUp.MethodWithVariousArgs().Returns(1);
        substitute.Object.MethodWithVariousArgs().Should().Be(1);
    }
}
