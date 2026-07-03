using System.Collections.Immutable;

using Keboo.SourceWeaver.Sdk.Output;
using Keboo.SourceWeaver.Sdk.Types;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Keboo.SourceWeaver.Sdk.Generators;

public abstract class AssemblyTypeGenerator : IIncrementalGenerator
{
    public abstract GenerationResult Generate(AssemblyTypeContext context);
    
    protected virtual bool IsTargetNode(SyntaxNode node, CancellationToken token) => 
        node.IsKind(SyntaxKind.ClassDeclaration) ||
        node.IsKind(SyntaxKind.StructDeclaration);

    protected virtual TypeDefinition? GetGenerationContext(GeneratorSyntaxContext context, CancellationToken token)
    {
        var symbol = context.SemanticModel.GetDeclaredSymbol(context.Node, token);
        return symbol is INamedTypeSymbol namedTypeSymbol ? new TypeDefinition(namedTypeSymbol) : null;
    }

    protected virtual string GetOutputHintName(AssemblyTypeContext context) => context.GetType().Name;

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var outputResults = context.SyntaxProvider
            .CreateSyntaxProvider(IsTargetNode, GetGenerationContext)
            .Where(static item => item is not null)
            .Collect()
            .SelectMany((items, _) =>
            {
                AssemblyTypeContext assemblyContext = new()
                {
                    AssemblyTypes = items
                };
                if (Generate(assemblyContext) is { IsSuccess: true })
                {
                    var rv = ImmutableArray.CreateBuilder<GenerationOutputResult>();

                    string hintName = GetOutputHintName(assemblyContext);

                    foreach (var outputItem in assemblyContext.GeneratedOutputs)
                    {
                        IndentingStringBuilder sb = new();
                        outputItem.WriteOutput(sb);
                        //TODO: Distinct hint names
                        rv.Add(new GenerationOutputResult(hintName, sb.ToString()));
                    }

                    return rv;
                }
                return [];
            })
            .Where(static outputResult => outputResult is not null);

        context.RegisterSourceOutput(outputResults,
            static (spc, outputResult) => spc.AddSource(outputResult!.OutputHintName, outputResult.GeneratedCode));
    }

    public TypeDefinition? GetTypeDefinition(string metadataName)
    {
        return null;
    }

}
