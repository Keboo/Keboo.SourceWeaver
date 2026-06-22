using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Keboo.SourceWeaver.Sdk;

public sealed class IndentingStringBuilder
{
    private static readonly Regex LineSplitRegex = new("\r?\n");

    private const int DefaultIndentSize = 4;

    public int IndentSize { get; set; } = DefaultIndentSize;

    public int IndentCount => _indentCount;

    private readonly StringBuilder _builder = new();
    private int _indentCount;

    public void Indent()
    {
        _indentCount++;
    }

    public void Outdent()
    {
        if (_indentCount == 0)
        {
            throw new InvalidOperationException("Cannot outdent. The indent count is already zero.");
        }

        _indentCount--;
    }

    public void AppendLine() => _builder.AppendLine();

    public void AppendLine(string value)
    {
        AppendIndent();
        _builder.AppendLine(value);
    }

    public void AppendLines(string lines)
    {
        var lineArray = LineSplitRegex.Split(lines);
        foreach (var line in lineArray)
        {
            AppendLine(line);
        }
    }

    public override string ToString()
    {
        return _builder.ToString();
    }

    private void AppendIndent()
    {
        _builder.Append(' ', IndentSize * _indentCount);
    }
}

