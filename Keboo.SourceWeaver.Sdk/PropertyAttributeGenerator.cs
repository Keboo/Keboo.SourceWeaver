namespace Keboo.SourceWeaver.Sdk;

public abstract class PropertyAttributeGenerator
{
    public abstract string AttributeName { get; }

    public abstract GenerationResult GenerateProperty(GenerationPropertyContext context);
}
