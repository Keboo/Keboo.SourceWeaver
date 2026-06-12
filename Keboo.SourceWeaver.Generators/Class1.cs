using Microsoft.CodeAnalysis;

namespace Keboo.SourceWeaver.Generators;

[Generator(LanguageNames.CSharp)]
public class InceptoSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        throw new NotImplementedException();
    }
}
