using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace GenSubstitute.Tests;

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
