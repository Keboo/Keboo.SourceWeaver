using Keboo.SourceWeaver.Sdk;

using Microsoft.CodeAnalysis;

namespace SampleApp.Generators;

[Generator(LanguageNames.CSharp)]
public class PropertyChangedGenerator : PropertyAttributeGenerator<PropertyChangedAttribute>
{
    public override GenerationResult Generate(GenerationPropertyContext context)
    {
        context.AddClassMember($$"""
            public partial {{context.PropertyType}} {{context.PropertyName}}
            {
                get => field;
                set
                {
                    if (field != value)
                    {
                        field = value;
                        PropertyChanged?.Invoke(this, new global::System.ComponentModel.PropertyChangedEventArgs(nameof({{context.PropertyName}})));
                    }
                }
            }
            """);
        return GenerationResult.Success;
    }
}
