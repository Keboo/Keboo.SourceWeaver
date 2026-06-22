namespace Keboo.SourceWeaver.Sdk;

public class GenerationOutput
{
    private readonly List<string> _usingStatements = [];
    public void AddUsing(string usingStatement) => _usingStatements.Add(usingStatement);
    public IReadOnlyList<string> UsingStatements => _usingStatements;

    private readonly List<string> _namespaceMembers = [];
    public void AddNamespaceMember(string member) => _namespaceMembers.Add(member);
    public IReadOnlyList<string> NamespaceMembers => _namespaceMembers;

    private readonly List<string> _classMembers = [];
    public void AddClassMember(string member) => _classMembers.Add(member);

    internal void WriteOutput(IndentingStringBuilder sb)
    {
        if (_classMembers.Count == 0 && NamespaceMembers.Count == 0)
        {
            return;
        }
        foreach (var usingStatement in UsingStatements)
        {
            sb.AppendLine(usingStatement);
        }

        if (Namespace is not null)
        {
            sb.AppendLine($"namespace {Namespace}");
            sb.AppendLine("{");
            sb.Indent();
        }

        foreach (var member in NamespaceMembers)
        {
            sb.AppendLines(member);
        }

        if (_classMembers.Count > 0 && ClassName is not null)
        {
            sb.AppendLine($"partial class {ClassName}");
            sb.AppendLine("{");
            sb.Indent();
            foreach (var member in _classMembers)
            {
                sb.AppendLines(member);
            }
            sb.Outdent();
            sb.AppendLine("}");
        }

        if (Namespace is not null)
        {
            sb.Outdent();
            sb.AppendLine("}");
        }
    }

    public IReadOnlyList<string> ClassMembers => _classMembers;

    public string? Namespace { get; set; }
    public string? ClassName { get; set; }
}
