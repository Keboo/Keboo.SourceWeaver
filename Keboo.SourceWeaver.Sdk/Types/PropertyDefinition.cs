

using Microsoft.CodeAnalysis;

namespace Keboo.SourceWeaver.Sdk.Types;

public class PropertyDefinition
{
    public PropertyDefinition(IPropertySymbol propertySymbol)
    {
        Name = propertySymbol.Name;
        PropertyType = new TypeDefinition(propertySymbol.Type);
        GenericTypeArguments = propertySymbol.Type is INamedTypeSymbol namedType && namedType.IsGenericType
            ? [..namedType.TypeArguments.Select(arg => new TypeDefinition(arg))]
            : [];
    }
    public string Name { get; }
    public TypeDefinition PropertyType { get; }
    public IReadOnlyList<TypeDefinition> GenericTypeArguments { get; }
}
