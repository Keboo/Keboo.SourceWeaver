namespace Keboo.SourceWeaver.Sdk;

public abstract class PropertyAttributeGenerator
{
    public abstract string AttributeName { get; }

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