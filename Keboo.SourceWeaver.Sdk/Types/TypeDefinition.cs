

using Microsoft.CodeAnalysis;

namespace Keboo.SourceWeaver.Sdk.Types;

public class TypeDefinition : IEquatable<TypeDefinition?>
{
    public TypeDefinition(ITypeSymbol typeSymbol)
    {
        Name = typeSymbol.Name;
        FullName = typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
    }

    public string FullName { get; }

    public string Name { get; }

    public IEnumerable<PropertyDefinition> Properties { get; set; } = [];

    public override bool Equals(object? obj)
    {
        return Equals(obj as TypeDefinition);
    }

    public bool Equals(TypeDefinition? other)
    {
        return other is not null &&
               FullName == other.FullName;
    }

    public override int GetHashCode()
    {
        return 733961487 + EqualityComparer<string>.Default.GetHashCode(FullName);
    }

    public static bool operator ==(TypeDefinition? left, TypeDefinition? right)
    {
        return EqualityComparer<TypeDefinition>.Default.Equals(left, right);
    }

    public static bool operator !=(TypeDefinition? left, TypeDefinition? right)
    {
        return !(left == right);
    }
}
