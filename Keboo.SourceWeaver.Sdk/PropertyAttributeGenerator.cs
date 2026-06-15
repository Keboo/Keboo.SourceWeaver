using System.Collections.Generic;

namespace Keboo.SourceWeaver.Sdk;

public abstract class PropertyAttributeGenerator<TAttribute>
    where TAttribute : Attribute
{
    protected string GetAttributeFullName() => typeof(TAttribute).FullName!;

    public abstract GenerationResult GenerateProperty(GenerationPropertyContext context);
}

public class GenerationClassContext
{
    public string? Namespace { get; init; }
    public string ClassName { get; init; } = "";
}

public class GenerationPropertyContext : GenerationClassContext
{
    private readonly List<string> _members = new List<string>();

    public string PropertyName { get; init; } = "";
    public string PropertyType { get; init; } = "";

    public void AddClassMember(string member) => _members.Add(member);

    public IReadOnlyList<string> Members => _members;
}