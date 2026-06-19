using System.Diagnostics;
using System.Text.RegularExpressions;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Keboo.SourceWeaver.Sdk;

public abstract class ClassAttributeGenerator<TAttribute> : IIncrementalGenerator
    where TAttribute : Attribute
{
    protected string GetAttributeFullName() => typeof(TAttribute).FullName!;

    public abstract GenerationResult Generate(GenerationClassContext context);

    private static readonly Regex LineSplitRegex = new("\r?\n");

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var classInfos = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                GetAttributeFullName(),
                static (node, _) => node is ClassDeclarationSyntax,
                static (ctx, _) =>
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
                    return default;
                })
            .Where(static item => item?.ClassName is not null);

        context.RegisterSourceOutput(classInfos, (spc, classContext) =>
        {
            Generate(classContext!);

            if (classContext!.NamespaceMembers.Count == 0)
                return;

            IndentingStringBuilder sb = new();

            foreach (var usingStatement in classContext.UsingStatements)
            {
                sb.AppendLine(usingStatement);
            }

            if (classContext.Namespace is not null)
            {
                sb.AppendLine($"namespace {classContext.Namespace}");
                sb.AppendLine("{");
                sb.Indent();
            }

            foreach (var member in classContext.NamespaceMembers)
            {
                foreach (var memberLine in LineSplitRegex.Split(member))
                {
                    sb.AppendLine(memberLine);
                }
            }

            if (classContext.Namespace is not null)
            {
                sb.Outdent();
                sb.AppendLine("}");
            }

            spc.AddSource($"{classContext.ClassName}_{typeof(TAttribute).Name}.g.cs", sb.ToString());
        });
    }
}