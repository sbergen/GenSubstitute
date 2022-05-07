using System.Linq;
using GenSubstitute.SourceGenerator;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace GenSubstitute.UnitTests;

public class SyntaxReceiverTests
{
    [Fact]
    public void CallsToGenSubstitute_AreGathered()
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(@"
using GenSubstitute;
Gen.Substitute<IFoo>();");

        var receiver = new SyntaxReceiver();
        foreach (var node in syntaxTree.GetRoot().DescendantNodes())
        {
            receiver.OnVisitSyntaxNode(node);
        }
        
        Assert.Equal(
            new[] { "IFoo" },
            receiver.TypesToMock.Select(syntax => syntax.ToString()));
    }
}