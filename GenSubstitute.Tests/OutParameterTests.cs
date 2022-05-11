using FluentAssertions;
using Xunit;

namespace GenSubstitute.Tests;

public static class OutParameterTests
{
    public interface IOutParams
    {
        bool TryGet(out int value);
    }

    [Fact]
    public static void OutParameter_CanBeMocked()
    {
        var builder = Gen.Substitute<IOutParams>().Build();

        builder.Configure.TryGet(Arg.Any).Configure(value =>
        {
            value.Value = 42;
            return true;
        });

        int value;
        builder.Object.TryGet(out value).Should().Be(true);
        value.Should().Be(42);
    }
}
