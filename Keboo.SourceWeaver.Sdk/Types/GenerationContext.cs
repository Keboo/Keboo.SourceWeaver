using Keboo.SourceWeaver.Sdk.Output;

namespace Keboo.SourceWeaver.Sdk.Types;

public class GenerationContext
{
    public NamespaceDefinition? Namespace { get; init; }

    private readonly List<GenerationOutput> _generatedOutputs = [];
    internal IReadOnlyList<GenerationOutput> GeneratedOutputs => _generatedOutputs;

    public void AddOutput(GenerationOutput output)
        => _generatedOutputs.Add(output);

    public virtual GenerationOutput CreateFromCurrent()
    {
        var rv = new GenerationOutput
        {
            Namespace = Namespace?.Namespace
        };
        AddOutput(rv);
        return rv;
    }
}
