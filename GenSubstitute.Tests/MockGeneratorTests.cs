using Xunit;

namespace GenSubstitute.Tests;

public class MockGeneratorTests
{
    [Fact]
    public void GeneratorTest()
    {
        var generatedCode = GeneratorUtility.GenerateCode(@"
using GenSubstitute;
Gen.Substitute<IFoo>();
");
        
        Assert.Contains("FOO", generatedCode);
    }
}
