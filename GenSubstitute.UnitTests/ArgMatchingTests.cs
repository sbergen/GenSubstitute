using FluentAssertions;
using Xunit;

namespace GenSubstitute.UnitTests;

public class ArgMatchingTests
{
    [Fact]
    public void RefArgsWrappedInArgs_Match()
    {
        // TODO, should we really have these?
        new Arg<RefArg<int>>(5).Matches(new RefArg<int>(5)).Should().BeTrue();
    }
}
