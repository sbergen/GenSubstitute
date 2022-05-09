using Xunit;

namespace GenSubstitute.Tests;

public class RefParametersTest
{
    public interface IRefParams
    {
        void Modify(ref int foo);
    }

    [Fact]
    public void InterfaceWithRefParameterMethod_ShouldBeMockable()
    {
        // This doesn't yet support doing anything with the ref arg.
        var builder = Gen.Substitute<IRefParams>().Build();
        int foo = 0;
        builder.Object.Modify(ref foo);
    }
}
