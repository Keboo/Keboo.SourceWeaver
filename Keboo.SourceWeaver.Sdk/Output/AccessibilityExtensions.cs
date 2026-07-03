using Microsoft.CodeAnalysis;

namespace Keboo.SourceWeaver.Sdk.Output;

public static class AccessibilityExtensions
{
    extension(Accessibility accessibility)
    {
        public string ToCSharpString()
        {
            return accessibility switch
            {
                Accessibility.Private => "private",
                Accessibility.ProtectedAndInternal => "protected internal",
                Accessibility.Protected => "protected",
                Accessibility.Internal => "internal",
                Accessibility.ProtectedOrInternal => "protected internal",
                Accessibility.Public => "public",
                _ => throw new ArgumentOutOfRangeException(nameof(accessibility), accessibility, null)
            };
        }
    }
}
