using FluentAssertions;
using GenSubstitute.SourceGenerator.Utilities;
using Xunit;

namespace GenSubstitute.UnitTests;

public static class LocalNameUniquifierTests
{
    [Fact]
    public static void UniqueNames_AreCorrectlyGenerated()
    {
        var uniquifier = new LocalNameUniquifier(new[]
        {
            "local_foo",
            "local_foo_1",
        });

        uniquifier.GetUniqueLocalName("foo").Should().Be("local_foo_2");
        uniquifier.GetUniqueLocalName("foo").Should().Be("local_foo_3");
    }
}
