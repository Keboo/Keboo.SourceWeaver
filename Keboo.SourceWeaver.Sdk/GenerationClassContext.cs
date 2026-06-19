namespace Keboo.SourceWeaver.Sdk;

public class GenerationClassContext
{
    public string? Namespace { get; init; }
    public string ClassName { get; init; } = "";

    private readonly List<string> _usingStatements = [];
    public void AddUsing(string usingStatement) => _usingStatements.Add(usingStatement);
    public IReadOnlyList<string> UsingStatements => _usingStatements;

    private readonly List<string> _namespaceMembers = [];
    public void AddNamespaceMember(string member) => _namespaceMembers.Add(member);
    public IReadOnlyList<string> NamespaceMembers => _namespaceMembers;
}
