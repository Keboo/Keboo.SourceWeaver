using System.Globalization;
using System.Text;

namespace Keboo.SourceWeaver.Sdk;

public sealed class IndentingStringBuilder
{
    private const int DefaultIndentSize = 4;

    public int IndentSize { get; set; } = DefaultIndentSize;

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

    public override string ToString()
    {
        return _builder.ToString();
    }

    private void AppendIndent()
    {
        _builder.Append(' ', IndentSize * _indentCount);
    }
}

