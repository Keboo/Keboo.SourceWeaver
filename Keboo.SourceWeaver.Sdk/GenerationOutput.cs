namespace Keboo.SourceWeaver.Sdk;

public class GenerationOutput
{
    private readonly List<string> _usingStatements = [];
    public IReadOnlyList<string> UsingStatements => _usingStatements;
    public void AddUsing(string usingStatement) => _usingStatements.Add(usingStatement);

    private readonly List<string> _namespaceMembers = [];
    public IReadOnlyList<string> NamespaceMembers => _namespaceMembers;
    public void AddNamespaceMember(string member) => _namespaceMembers.Add(member);
    public void AddNamespaceMember(WhitespaceInterpolatedStringHandler member) => _namespaceMembers.Add(member.ToString());

    private readonly List<string> _classMembers = [];
    public IReadOnlyList<string> ClassMembers => _classMembers;
    public void AddClassMember(string member) => _classMembers.Add(member);
    public void AddClassMember(WhitespaceInterpolatedStringHandler member) => _classMembers.Add(member.ToString());

    internal void WriteOutput(IndentingStringBuilder sb)
    {
        if (ClassMembers.Count == 0 && NamespaceMembers.Count == 0)
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

        if (ClassMembers.Count > 0 && ClassName is not null)
        {
            sb.AppendLine($"partial class {ClassName}");
            sb.AppendLine("{");
            sb.Indent();
            foreach (var member in ClassMembers)
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

    public string? Namespace { get; set; }
    public string? ClassName { get; set; }
}
