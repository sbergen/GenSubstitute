using Xunit;

namespace GenSubstitute.UnitTests;

public class MockGeneratorTests
{
    // TODO: I should really split this up into multiple tests...
    [Fact]
    public void GeneratorSmokeTest()
    {
        GeneratorUtility.AssertNoInspections(@"
using GenSubstitute;
using System;
using Some.Nested.Namespace;

var builder = Gen.Substitute<SomeClass.SomeNestedClass.IFoo<int>>().Build();
var builder2 = Gen.Substitute<SomeClass.SomeNestedClass.IFoo<int>>().Build();

var externalBuilder = Gen.Substitute<IDisposable>().Build();

namespace Some.Nested.Namespace
{
    static class SomeClass
    {
        public static class SomeNestedClass
        {
            public interface IFoo<T>
            {
                int Foo(ref int arg);
                void Bar(); 
                T2 Generic<T2>();
                double MultipleArgs(int i1, int i2, int i3 = 42);
            }
        }
    }
}
");
    }
}
