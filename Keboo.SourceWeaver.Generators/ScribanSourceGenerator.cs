using System.Diagnostics;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Keboo.SourceWeaver.Generators;

[Generator(LanguageNames.CSharp)]
public class ScribanSourceGenerator : IIncrementalGenerator
{
    private class ScribanFile
    {
        public string Name { get; }
        public string Content { get; }

        public ScribanFile(string name, string content)
        {
            Name = name;
            Content = content;
        }
    }

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Check if Microsoft.Extensions.Diagnostics.Testing is referenced
        IncrementalValuesProvider<AdditionalText> scribanFiles = context.AdditionalTextsProvider.Where(
            static file => file.Path.EndsWith(".scriban"));

        IncrementalValuesProvider<(string name, string content)> namesAndContents = scribanFiles.Select((text, cancellationToken) => (name: Path.GetFileNameWithoutExtension(text.Path), content: text.GetText(cancellationToken)!.ToString()));

        IncrementalValuesProvider<ScribanFile> templates = scribanFiles
            .Select(static (additionalText, token) => GetScribanTemplates(additionalText, token));

        // Only generate source if enabled and Microsoft.Extensions.Diagnostics.Testing is referenced
        context.RegisterSourceOutput(templates, static (context, scribanFile) =>
        {
            if (!string.IsNullOrEmpty(scribanFile.Content))
            {
                context.AddSource($"{scribanFile.Name}.g.cs", BuildScribanSourceGenerator(scribanFile));
            }
        });

    }

    private static ScribanFile GetScribanTemplates(AdditionalText additionalText, CancellationToken cancellationToken)
    {
        return new ScribanFile(
            Path.GetFileNameWithoutExtension(additionalText.Path), 
            additionalText.GetText(cancellationToken)!.ToString());
    }

    private static string BuildScribanSourceGenerator(ScribanFile scribanFile)
    {
        return $$""""
        using Microsoft.CodeAnalysis;
        using Microsoft.CodeAnalysis.CSharp;
        using Microsoft.CodeAnalysis.CSharp.Syntax;

        namespace SampleApp.Generators;

        [Generator(LanguageNames.CSharp)]
        public class FooGenerator : IIncrementalGenerator
        {
            private const string Template = """
            {{scribanFile.Content}}
        """;


            /// <summary>
            /// Checks whether a given <see cref="PropertyDeclarationSyntax"/> has or could possibly have any attributes, using only syntax.
            /// </summary>
            /// <param name="typeDeclaration">The input <see cref="PropertyDeclarationSyntax"/> instance to check.</param>
            /// <returns>Whether <paramref name="typeDeclaration"/> has or could possibly have any attributes.</returns>
            private static bool HasOrPotentiallyHasAttributes(PropertyDeclarationSyntax typeDeclaration)
            {
                // If the type has any attributes lists, then clearly it can have attributes
                if (typeDeclaration.AttributeLists.Count > 0)
                {
                    return true;
                }

                // If the declaration has no attribute lists, check if the type is partial. If it is, it means
                // that there could be another partial declaration with some attribute lists over them.
                foreach (SyntaxToken modifier in typeDeclaration.Modifiers)
                {
                    if (modifier.IsKind(SyntaxKind.PartialKeyword))
                    {
                        return true;
                    }
                }

                return false;
            }

            public void Initialize(IncrementalGeneratorInitializationContext context)
            {
                System.Diagnostics.Debugger.Launch();
                // Gather all generation info, and any diagnostics
                IncrementalValuesProvider<(string?, string?)> propertyInfos =
                    context.SyntaxProvider
                    .ForAttributeWithMetadataName(
                        "SampleApp.Generators.PropertyChangedAttribute",
                        static (node, _) => node is PropertyDeclarationSyntax propertyDeclaration && 
                                HasOrPotentiallyHasAttributes(propertyDeclaration),
                        (context, token) =>
                        {
                            if(context.TargetSymbol is IPropertySymbol propertySymbol)
                            {
                                return (propertySymbol.ContainingType.Name, propertySymbol.Name);
                            }

                            return default;
                        })
                    .Where(static item => item is { Item1: not null, Item2: not null })!;


                // Generate the required members
                context.RegisterSourceOutput(propertyInfos, (context, item) =>
                {
                    var template = Scriban.Template.Parse(Template);
                    var result = template.Render(new { ClassName = item.Item1, PropertyName = item.Item2 });

                    context.AddSource($"{item.Item1}_{item.Item2}.g.cs", result);
                });
            }
        }
        """";
    }
}

[Generator(LanguageNames.CSharp)]
public class FooGenerator : IIncrementalGenerator
{
    private const string Template = """
    partial class {{ClassName}}
    {
        public partial string {{PropertyName}}
        {
            get => field;
            set
            {
                if (field != value)
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof({{PropertyName}})));
                }
            }
        }
    }
    """;


    /// <summary>
    /// Checks whether a given <see cref="PropertyDeclarationSyntax"/> has or could possibly have any attributes, using only syntax.
    /// </summary>
    /// <param name="typeDeclaration">The input <see cref="PropertyDeclarationSyntax"/> instance to check.</param>
    /// <returns>Whether <paramref name="typeDeclaration"/> has or could possibly have any attributes.</returns>
    private static bool HasOrPotentiallyHasAttributes(PropertyDeclarationSyntax typeDeclaration)
    {
        // If the type has any attributes lists, then clearly it can have attributes
        if (typeDeclaration.AttributeLists.Count > 0)
        {
            return true;
        }

        // If the declaration has no attribute lists, check if the type is partial. If it is, it means
        // that there could be another partial declaration with some attribute lists over them.
        foreach (SyntaxToken modifier in typeDeclaration.Modifiers)
        {
            if (modifier.IsKind(SyntaxKind.PartialKeyword))
            {
                return true;
            }
        }

        return false;
    }

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Gather all generation info, and any diagnostics
        IncrementalValuesProvider<(string?, string?)> propertyInfos =
            context.SyntaxProvider
            .ForAttributeWithMetadataName(
                "SampleApp.Generators.PropertyChangedAttribute",
                static (node, _) => node is PropertyDeclarationSyntax propertyDeclaration && 
                        HasOrPotentiallyHasAttributes(propertyDeclaration),
                (context, token) =>
                {
                    if(context.TargetSymbol is IPropertySymbol propertySymbol)
                    {
                        return (propertySymbol.ContainingType.Name, propertySymbol.Name);
                    }

                    return default;
                })
            .Where(static item => item is { Item1: not null, Item2: not null })!;


        // Generate the required members
        context.RegisterSourceOutput(propertyInfos, (context, item) =>
        {
            var template = Scriban.Template.Parse(Template);
            var result = template.Render(new { ClassName = item.Item1, PropertyName = item.Item2 });

            context.AddSource($"{item.Item1}_{item.Item2}.g.cs", result);
        });
    }
}
