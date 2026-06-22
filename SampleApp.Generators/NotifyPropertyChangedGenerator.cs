using Keboo.SourceWeaver.Sdk;

using Microsoft.CodeAnalysis;

namespace SampleApp.Generators;

[Generator(LanguageNames.CSharp)]
public class NotifyPropertyChangedGenerator : ClassAttributeGenerator<NotifyPropertyChangedAttribute>
{
    public override GenerationResult Generate(GenerationClassContext context)
    {
        if (context.ClassName.EndsWith("Foo"))
        {
            return GenerationResult.Skip;
        }

        GenerationOutput output = context.FromCurrent();
        output.AddUsing("using System.ComponentModel;");
        output.AddNamespaceMember($$"""
            partial class {{context.ClassName}} : INotifyPropertyChanged
            {
                public event PropertyChangedEventHandler? PropertyChanged;
            }
            """);

        return GenerationResult.Success;
    }
}
