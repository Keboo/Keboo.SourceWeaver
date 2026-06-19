namespace Keboo.SourceWeaver.Sdk;

public class GenerationPropertyContext : GenerationClassContext
{
    private readonly List<string> _classMembers = [];

    public string PropertyName { get; init; } = "";
    public string PropertyType { get; init; } = "";

    public void AddClassMember(string member) => _classMembers.Add(member);

    public IReadOnlyList<string> ClassMembers => _classMembers;
}