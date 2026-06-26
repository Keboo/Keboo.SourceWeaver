using System.Collections.Immutable;

using Microsoft.CodeAnalysis;

namespace Keboo.SourceWeaver.Sdk;

public abstract class AttributeGenerator<TAttribute, TContext> : IIncrementalGenerator
    where TAttribute : Attribute
    where TContext : GenerationContext
{
    private protected AttributeGenerator() { }

    protected string GetAttributeFullName() => typeof(TAttribute).FullName!;

    protected abstract string GetOutputHintName(TContext context);

    public abstract GenerationResult Generate(TContext context);

    protected abstract TContext? GetGenerationContext(GeneratorAttributeSyntaxContext ctx, CancellationToken token);

    protected abstract bool IsTargetNode(SyntaxNode node, CancellationToken token);

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var outputResults = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                GetAttributeFullName(),
                IsTargetNode,
                GetGenerationContext)
            .Where(static item => item is not null)
            .SelectMany((item, _) =>
            {
                if (Generate(item!) is { IsSuccess: true })
                {
                    var rv = ImmutableArray.CreateBuilder<GenerationOutputResult>();

                    string hintName = GetOutputHintName(item!);

                    foreach(var outputItem in item!.GeneratedOutputs)
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

    private record class GenerationOutputResult(string OutputHintName, string GeneratedCode);
}
