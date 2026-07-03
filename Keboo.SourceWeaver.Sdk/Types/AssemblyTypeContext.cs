namespace Keboo.SourceWeaver.Sdk.Types;

public class AssemblyTypeContext : GenerationContext
{
    public IEnumerable<TypeDefinition> AssemblyTypes { get; set; } = [];
}
