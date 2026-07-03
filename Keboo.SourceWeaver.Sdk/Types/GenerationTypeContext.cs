using Keboo.SourceWeaver.Sdk.Output;

namespace Keboo.SourceWeaver.Sdk.Types;


public class GenerationTypeContext : GenerationContext
{
    public required TypeDefinition Type { get; init; }

    public override GenerationOutput CreateFromCurrent()
    {
        var rv = base.CreateFromCurrent();
        rv.TypeName = Type?.Name;
        return rv;
    }
}
