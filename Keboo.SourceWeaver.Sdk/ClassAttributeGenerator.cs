using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Keboo.SourceWeaver.Sdk;

public abstract class ClassAttributeGenerator<TAttribute> : 
    AttributeGenerator<TAttribute, GenerationClassContext>
    where TAttribute : Attribute
{
    protected override string GetOutputHintName(GenerationClassContext context)
        => $"{context.ClassName}_{typeof(TAttribute).Name}.g.cs";

    protected override bool IsTargetNode(SyntaxNode node, CancellationToken token)
        => node is ClassDeclarationSyntax;

    protected override GenerationClassContext? GetGenerationContext(GeneratorAttributeSyntaxContext ctx, CancellationToken token)
    {
        if (ctx.TargetSymbol is ITypeSymbol typeSymbol)
        {
            string? ns = typeSymbol.ContainingNamespace?.IsGlobalNamespace == true
                ? null
                : typeSymbol.ContainingNamespace?.ToDisplayString();
            return new GenerationClassContext
            {
                Namespace = ns,
                ClassName = typeSymbol.Name
            };
        }
        return null;
    }
}