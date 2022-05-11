using FluentAssertions;
using Xunit;

namespace GenSubstitute.Tests;

public class RefParametersTest
{
    public interface IRefParams
    {
        void Modify(int val, ref int reference);
    }

    [Fact]
    public void InterfaceWithRefParameterMethod_ShouldBeMockable()
    {
        var builder = Gen.Substitute<IRefParams>().Build();
        
        builder.Configure.Modify(Arg.Any, 0)
            .Configure((i, r) => r.Value = i);
        
        int foo = 0;
        builder.Object.Modify(42, ref foo);
        foo.Should().Be(42);
    }
}
