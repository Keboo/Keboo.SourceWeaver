namespace Keboo.SourceWeaver.Sdk;

public abstract class PropertyAttributeGenerator<TAttribute> 
    where TAttribute : Attribute
{
    public abstract GenerationResult GenerateProperty(GenerationPropertyContext context);
}

public class GenerationClassContext
{
    public string ClassName { get; init; }
}

public class GenerationPropertyContext : GenerationClassContext
{
    public string PropertyName { get; init; }
}