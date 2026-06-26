using System.Runtime.CompilerServices;
using System.Text;

namespace Keboo.SourceWeaver.Sdk;

[InterpolatedStringHandler]
public readonly struct WhitespaceInterpolatedStringHandler
{
    // Storage for the built-up string
    private readonly StringBuilder _builder;

    public WhitespaceInterpolatedStringHandler(int literalLength, int formattedCount)
    {
        _builder = new StringBuilder(literalLength);
    }

    public void AppendLiteral(string s)
    {
        _builder.Append(s);
    }

    public readonly void AppendFormatted<T>(T? value)
    {
        _builder.Append(value);
    }

    public readonly void AppendFormatted<T>(T? value, string? format)
    {
        if (value is not null)
        {
            if (format == "+")
            {
                var previousLength = _builder.Length;
                _builder.Append(value);
                if (_builder.Length > previousLength)
                {
                    _builder.Append(" ");
                }
            }
            else
            {
                _builder.Append(value);
            }
        }
    }

    public override readonly string ToString() => _builder.ToString();
}
