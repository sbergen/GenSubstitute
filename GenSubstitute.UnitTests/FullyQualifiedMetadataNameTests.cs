using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;
using GenSubstitute.SourceGenerator.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace GenSubstitute.UnitTests;

public class FullyQualifiedMetadataNameTests
{
    [Fact]
    public void Type_CanBeResolvedWithResolvedName_WhenNonNestedNonGenericType() =>
        ResolveSymbolByName("class SomeClass {}", "SomeClass").Should().NotBeNull();
    
    [Fact]
    public void Type_CanBeResolvedWithResolvedName_WhenNonNestedGenericType() =>
        ResolveSymbolByName("class SomeClass<T> {}", "SomeClass").Should().NotBeNull();

    [Fact]
    public void Type_CanBeResolvedWithResolvedName_WhenDoublyNestedNonGenericType() =>
        ResolveSymbolByName(
            "class ContainingClass { class NestedClass { class MostNestedClass {} } }", 
            "MostNestedClass")
            .Should()
            .NotBeNull();
    
    [Fact]
    public void Type_CanBeResolvedWithResolvedName_WhenDoublyNestedGenericInterface() =>
        ResolveSymbolByName(
                "class ContainingClass<T1> { class NestedClass<T2> { interface MostNestedInterface<T3> {} } }", 
                "MostNestedInterface")
            .Should()
            .NotBeNull();
    
    [Fact]
    public void Type_CanBeResolvedWithResolvedName_WhenDoublyNestedGenericType() =>
        ResolveSymbolByName(
                "class ContainingClass<T> { class NestedClass<T2> { class MostNestedClass<T3> {} } }", 
                "MostNestedClass")
            .Should()
            .NotBeNull();
    
    [Fact]
    public void Type_CanBeResolvedWithResolvedName_WhenNestedNamespacesAndTypes() =>
        ResolveSymbolByName(
                "namespace ContainingNamespace { namespace NestedNamespace { class ContainingClass<T> { class NestedClass { class MostNestedClass<T> {} } } } }", 
                "MostNestedClass")
            .Should()
            .NotBeNull();
    
    private static INamedTypeSymbol? ResolveSymbolByName(string source, string simpleName)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source);
        var compilation = syntaxTree.CreateCompilation();
        var declaration = FindTypeDeclaration(syntaxTree, simpleName);
        
        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var name = semanticModel.GetDeclaredSymbol(declaration)!.FullyQualifiedMetadataName();
        return compilation.GetTypeByMetadataName(name);
    }

    private static TypeDeclarationSyntax FindTypeDeclaration(SyntaxTree tree, string name)
    {
        var collector = new ClassDeclarationCollector();
        collector.Visit(tree.GetRoot());
        return collector.MatchingNodes.Single(n => n.Identifier.Text == name);
    }
    
    private class ClassDeclarationCollector : CSharpSyntaxWalker
    {
        public readonly List<TypeDeclarationSyntax> MatchingNodes = new ();
        
        public override void DefaultVisit(SyntaxNode node)
        {
            if (node is TypeDeclarationSyntax classDeclarationSyntax)
            {
                MatchingNodes.Add(classDeclarationSyntax);
            }

            base.DefaultVisit(node);
        }
    }
}
