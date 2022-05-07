using Xunit;

namespace GenSubstitute.UnitTests;

public class MockGeneratorTests
{
    [Fact]
    public void GeneratorSmokeTest()
    {
        GeneratorUtility.AssertNoInspections(@"
using GenSubstitute;

var builder = Gen.Substitute<IFoo>().Build();
var builder2 = Gen.Substitute<IFoo>().Build();

interface IFoo
{
    int Foo(int arg);
    void Bar(); 
    T Generic<T>();
    double MultipleArgs(int i1, int i2, int i3);
}
");
    }
}
