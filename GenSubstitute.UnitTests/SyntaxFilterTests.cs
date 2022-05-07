using System.Collections.Generic;
using FluentAssertions;
using GenSubstitute.SourceGenerator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace GenSubstitute.UnitTests;

public class SyntaxFilterTests
{
    private const string DefaultSource = @"
using GenSubstitute;

class TestClass
{
    public void SomeTest()
    {
        var mock = Gen.Substitute<IFoo>().Build();
    }
}";
    
    [Fact]
    public void ValidSubstituteCall_IsIdentified()
    {
        GetMatchingNodes(DefaultSource).Should().HaveCount(1);
    }
    
    [Fact]
    public void SubstitutedType_IsPropertyExtracted()
    {
        var typeSyntax = SyntaxFilter.ExtractTypeFromSubstituteCall(GetMatchingNodes(DefaultSource)[0]);
        typeSyntax?.ToString().Should().Be("IFoo");
    }

    private static List<SyntaxNode> GetMatchingNodes(string source)
    {
        var root = CSharpSyntaxTree.ParseText(source).GetCompilationUnitRoot();
        var collector = new SubstituteCallCollector();
        collector.Visit(root);
        return collector.MatchingNodes;
    }

    class SubstituteCallCollector : CSharpSyntaxWalker
    {
        public readonly List<SyntaxNode> MatchingNodes = new ();
        
        public override void DefaultVisit(SyntaxNode node)
        {
            if (SyntaxFilter.IsSubstituteCall(node))
            {
                MatchingNodes.Add(node);
            }
            
            base.DefaultVisit(node);
        }
    }
}
