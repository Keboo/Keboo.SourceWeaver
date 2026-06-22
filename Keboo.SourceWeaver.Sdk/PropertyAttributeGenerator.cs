using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Keboo.SourceWeaver.Sdk;

public abstract class PropertyAttributeGenerator<TAttribute> : 
    AttributeGenerator<TAttribute, GenerationPropertyContext>
    where TAttribute : Attribute
{
    protected override string GetOutputHintName(GenerationPropertyContext context)
        => $"{context.ClassName}_{context.PropertyName}_{typeof(TAttribute).Name}.g.cs";

    protected override bool IsTargetNode(SyntaxNode node, CancellationToken token)
        => node is PropertyDeclarationSyntax;

    protected override GenerationPropertyContext? GetGenerationContext(GeneratorAttributeSyntaxContext ctx, CancellationToken token)
    {
        if (ctx.TargetSymbol is IPropertySymbol propertySymbol)
        {
            var containingType = propertySymbol.ContainingType;
            string? ns = containingType.ContainingNamespace?.IsGlobalNamespace == true
                ? null
                : containingType.ContainingNamespace?.ToDisplayString();
            return new GenerationPropertyContext
            {
                Namespace = ns,
                ClassName = containingType.Name,
                PropertyName = propertySymbol.Name,
                PropertyType = propertySymbol.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
            };
        }
        return null;
    }
}
