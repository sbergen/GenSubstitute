using FluentAssertions;
using Xunit;

namespace GenSubstitute.Tests;

public class ArgMatchingTests
{
    [Fact]
    public void RefArg_MatchesValue()
    {
        new Arg<Ref<int>>(5).Matches(5).Should().BeTrue();
    }

    // TODO, should probably write more tests here
}
