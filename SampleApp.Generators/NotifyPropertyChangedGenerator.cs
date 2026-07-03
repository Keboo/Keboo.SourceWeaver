using Keboo.SourceWeaver.Sdk.Generators;
using Keboo.SourceWeaver.Sdk.Output;
using Keboo.SourceWeaver.Sdk.Types;

using Microsoft.CodeAnalysis;

namespace SampleApp.Generators;

[Generator(LanguageNames.CSharp)]
public class NotifyPropertyChangedGenerator : ClassAttributeGenerator<NotifyPropertyChangedAttribute>
{
    public override GenerationResult Generate(GenerationTypeContext context)
    {
        if (context.Type.Name.EndsWith("Foo"))
        {
            return GenerationResult.Skip;
        }

        GenerationOutput output = context.CreateFromCurrent();
        output.AddUsing("using System.ComponentModel;");
        output.AddNamespaceMember($$"""
            partial class {{context.Type.Name}} : INotifyPropertyChanged
            {
                public event PropertyChangedEventHandler? PropertyChanged;
            }
            """);

        return GenerationResult.Success;
    }
}
