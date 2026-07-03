namespace Keboo.SourceWeaver.Sdk.Types;

public class GenerationPropertyContext : GenerationTypeContext
{
    public string PropertyAccessModifier { get; init; } = "";
    public string PropertyName { get; init; } = "";
    public string PropertyType { get; init; } = "";
}
