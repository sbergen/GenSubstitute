using Xunit;

namespace GenSubstitute.UnitTests;

public class MockGeneratorTests
{
    [Fact]
    public void GeneratorSmokeTest()
    {
        GeneratorUtility.AssertNoInspections(@"
using GenSubstitute;

var builder = Gen.Substitute<IFoo<int>>().Build();
var builder2 = Gen.Substitute<IFoo<int>>().Build();

namespace GenSubstitute
{
interface IFoo<T>
{
    int Foo(int arg);
    void Bar(); 
    T2 Generic<T2>();
    double MultipleArgs(int i1, int i2, int i3);
}
}
");
    }
}
