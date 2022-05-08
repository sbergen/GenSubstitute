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
        var builder = Gen.Substitute<IWithDefaultArguments>().Build();
        builder.MethodWithVariousArgs().Returns(1);
        builder.Object.MethodWithVariousArgs(TypeKind.Array, "foo").Should().Be(1);
    }
    
}
