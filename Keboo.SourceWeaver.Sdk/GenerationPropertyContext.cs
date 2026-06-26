namespace Keboo.SourceWeaver.Sdk;

public class GenerationPropertyContext : GenerationClassContext
{
    public string PropertyAccessModifier { get; init; } = "";
    public string PropertyName { get; init; } = "";
    public string PropertyType { get; init; } = "";
}
