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
                PropertyAccessModifier = GetActualAccessModifier(propertySymbol, token),
                PropertyName = propertySymbol.Name,
                PropertyType = propertySymbol.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
            };
        }
        return null;
    }

    /// <summary>
    /// When a property does not specify an access modifier the DeclaredAccessibility is still reported as private.
    /// This is a problem for partial properties where the access modifier must always match.
    /// </summary>
    /// <returns></returns>
    private static string GetActualAccessModifier(IPropertySymbol propertySymbol, CancellationToken token)
    {
        string? accessModifier = null;
        if (propertySymbol.DeclaredAccessibility == Accessibility.Private)
        {
            foreach (var syntaxReference in propertySymbol.DeclaringSyntaxReferences)
            {
                if (syntaxReference.GetSyntax(token) is PropertyDeclarationSyntax propSyntax)
                {
                    foreach (var modifier in propSyntax.Modifiers)
                    {
                        if (modifier.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.PrivateKeyword))
                        {
                            accessModifier = Accessibility.Private.ToCSharpString();
                            break;
                        }
                    }
                }
            }
        }
        else
        {
            accessModifier = propertySymbol.DeclaredAccessibility.ToCSharpString();
        }
        return accessModifier ?? "";
    }
}
