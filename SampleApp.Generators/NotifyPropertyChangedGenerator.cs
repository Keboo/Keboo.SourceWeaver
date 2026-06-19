using Keboo.SourceWeaver.Sdk;

using Microsoft.CodeAnalysis;

namespace SampleApp.Generators;

[Generator(LanguageNames.CSharp)]
public class NotifyPropertyChangedGenerator : ClassAttributeGenerator<NotifyPropertyChangedAttribute>
{
    public override GenerationResult Generate(GenerationClassContext context)
    {
        context.AddUsing("using System.ComponentModel;");

        context.AddNamespaceMember($$"""
            partial class {{context.ClassName}} : INotifyPropertyChanged
            {
                public event PropertyChangedEventHandler? PropertyChanged;
            }
            """);
        return GenerationResult.Success;
    }
}
