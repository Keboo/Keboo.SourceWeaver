using Keboo.SourceWeaver.Sdk;

using Microsoft.CodeAnalysis;

namespace SampleApp.Generators;

public partial class PropertyChangedGenerator : PropertyAttributeGenerator<PropertyChangedAttribute>
{
    public override GenerationResult GenerateProperty(GenerationPropertyContext context)
    {
        context.AddClassMember($$"""
            public partial string {{context.PropertyName}}
            {
                get => field;
                set
                {
                    if (field != value)
                    {
                        field = value;
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof({{context.PropertyName}})));
                    }
                }
            }
            """);
        return GenerationResult.Success;
    }
}

public partial class PropertyChangedGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        
    }
}
