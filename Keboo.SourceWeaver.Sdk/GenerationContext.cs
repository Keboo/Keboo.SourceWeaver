namespace Keboo.SourceWeaver.Sdk;

public class GenerationContext
{
    public string? Namespace { get; init; }

    private readonly List<GenerationOutput> _generatedOutputs = [];
    internal IReadOnlyList<GenerationOutput> GeneratedOutputs => _generatedOutputs;

    public void AddOutput(GenerationOutput output)
        => _generatedOutputs.Add(output);

    public virtual GenerationOutput FromCurrent()
    {
        var rv = new GenerationOutput
        {
            Namespace = Namespace
        };
        AddOutput(rv);
        return rv;
    }
}
