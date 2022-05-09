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
        // This doesn't yet support doing anything with the ref arg.
        var builder = Gen.Substitute<IRefParams>().Build();
        
        builder.Configure.Modify(0, 0)
            .Configure((i, r) => r.Value = 42);
        
        int foo = 0;
        builder.Object.Modify(0, ref foo);
        foo.Should().Be(42);
    }
}
