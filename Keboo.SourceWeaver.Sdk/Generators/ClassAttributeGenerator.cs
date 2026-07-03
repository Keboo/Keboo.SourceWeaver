using Keboo.SourceWeaver.Sdk.Types;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Keboo.SourceWeaver.Sdk.Generators;

public abstract class ClassAttributeGenerator<TAttribute> : 
    AttributeGenerator<TAttribute, GenerationTypeContext>
    where TAttribute : Attribute
{
    protected override string GetOutputHintName(GenerationTypeContext context)
        => $"{context.Type!.Name}_{typeof(TAttribute).Name}.g.cs";

    protected override bool IsTargetNode(SyntaxNode node, CancellationToken token)
        => node is ClassDeclarationSyntax;

    protected override GenerationTypeContext? GetGenerationContext(GeneratorAttributeSyntaxContext ctx, CancellationToken token)
    {
        if (ctx.TargetSymbol is ITypeSymbol typeSymbol)
        {
            return new GenerationTypeContext
            {
                Namespace = new NamespaceDefinition(typeSymbol.ContainingNamespace),
                Type = new TypeDefinition(typeSymbol)
            };
        }
        return null;
    }
}