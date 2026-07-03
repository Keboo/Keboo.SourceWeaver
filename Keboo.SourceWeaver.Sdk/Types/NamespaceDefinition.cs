using Microsoft.CodeAnalysis;

namespace Keboo.SourceWeaver.Sdk.Types;

public class NamespaceDefinition
{
    public NamespaceDefinition(INamespaceSymbol? symbol) 
    {
        Namespace = symbol?.IsGlobalNamespace == true
            ? ""
            : symbol?.ToDisplayString() ?? "";
    }

    public string Namespace { get; set; }
}
