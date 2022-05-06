using FluentAssertions;
using Xunit;

namespace GenSubstitute.Tests;

public class MockGeneratorTests
{
    [Fact]
    public void GeneratorTest()
    {
        var generatedCode = GeneratorUtility.GenerateCode(@"
using GenSubstitute;

var mock = Gen.Substitute<IFoo>().Configure();

interface IFoo
{
}
");

        generatedCode.ToString().Should().Contain("Configure");
    }
}
