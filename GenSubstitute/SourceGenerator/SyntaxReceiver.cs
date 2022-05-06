using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace GenSubstitute.SourceGenerator
{
    internal class SyntaxReceiver : ISyntaxReceiver
    {
        private readonly List<TypeSyntax> _typesToMock = new ();
        public IReadOnlyList<TypeSyntax> TypesToMock => _typesToMock;

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is IdentifierNameSyntax
                {
                    Identifier: { ValueText: nameof(Gen) },
                    Parent: MemberAccessExpressionSyntax memberAccessSyntax
                })
            {
                // TODO, this could be optimized later
                foreach (var child in memberAccessSyntax.ChildNodes())
                {
                    if (child is GenericNameSyntax genericNameSyntax &&
                        genericNameSyntax.Arity == 1 &&
                        genericNameSyntax.Identifier.ValueText == nameof(Gen.Substitute))
                    {
                        _typesToMock.Add(genericNameSyntax.TypeArgumentList.Arguments[0]);
                        break;
                    }
                }
            }
        }
    }
}
