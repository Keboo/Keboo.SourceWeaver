using System.Text.RegularExpressions;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Keboo.SourceWeaver.Sdk;

public abstract class PropertyAttributeGenerator<TAttribute> : IIncrementalGenerator
    where TAttribute : Attribute
{
    protected string GetAttributeFullName() => typeof(TAttribute).FullName!;

    public abstract GenerationResult Generate(GenerationPropertyContext context);

    private static readonly Regex LineSplitRegex = new("\r?\n");

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var propertyInfos = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                GetAttributeFullName(),
                static (node, _) => node is PropertyDeclarationSyntax,
                static (ctx, _) =>
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
                    return default;
                })
            .Where(static item => item?.ClassName is not null);

        context.RegisterSourceOutput(propertyInfos, (spc, propertyContext) =>
        {
            Generate(propertyContext!);

            if (propertyContext!.ClassMembers.Count == 0)
                return;

            IndentingStringBuilder sb = new();

            foreach (var usingStatement in propertyContext.UsingStatements)
            {
                sb.AppendLine(usingStatement);
            }

            if (propertyContext.Namespace is not null)
            {
                sb.AppendLine($"namespace {propertyContext.Namespace}");
                sb.AppendLine("{");
                sb.Indent();
            }

            sb.AppendLine($"partial class {propertyContext.ClassName}");
            sb.AppendLine("{");
            sb.Indent();


            foreach (var member in propertyContext.ClassMembers)
            {
                foreach (var memberLine in LineSplitRegex.Split(member))
                {
                    sb.AppendLine(memberLine);
                }
            }

            sb.Outdent();
            sb.AppendLine("}");

            if (propertyContext.Namespace is not null)
            {
                sb.Outdent();
                sb.AppendLine("}");
            }

            spc.AddSource($"{propertyContext.ClassName}_{propertyContext.PropertyName}_{typeof(TAttribute).Name}.g.cs", sb.ToString());
        });
    }
}
