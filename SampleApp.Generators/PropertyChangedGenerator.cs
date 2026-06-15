using Keboo.SourceWeaver.Sdk;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SampleApp.Generators;

[Generator(LanguageNames.CSharp)]
public partial class PropertyChangedGenerator : PropertyAttributeGenerator<PropertyChangedAttribute>
{
    public override GenerationResult GenerateProperty(GenerationPropertyContext context)
    {
        context.AddClassMember($$"""
            public partial {{context.PropertyType}} {{context.PropertyName}}
            {
                get => field;
                set
                {
                    if (field != value)
                    {
                        field = value;
                        PropertyChanged?.Invoke(this, new global::System.ComponentModel.PropertyChangedEventArgs(nameof({{context.PropertyName}})));
                    }
                }
            }
            """);
        return GenerationResult.Success;
    }
}

public partial class PropertyChangedGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var propertyInfos = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                GetAttributeFullName(),
                static (node, _) => node is PropertyDeclarationSyntax,
                static (ctx, _) =>
                {
                    if (ctx.TargetSymbol is IPropertySymbol propertySymbol)
                    {
                        var containingType = propertySymbol.ContainingType;
                        string? ns = containingType.ContainingNamespace?.IsGlobalNamespace == true
                            ? null
                            : containingType.ContainingNamespace?.ToDisplayString();
                        return (
                            Namespace: ns,
                            ClassName: containingType.Name,
                            PropertyName: propertySymbol.Name,
                            PropertyType: propertySymbol.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
                        );
                    }
                    return default;
                })
            .Where(static item => item.ClassName is not null);

        context.RegisterSourceOutput(propertyInfos, (spc, item) =>
        {
            var propertyContext = new GenerationPropertyContext
            {
                Namespace = item.Namespace,
                ClassName = item.ClassName!,
                PropertyName = item.PropertyName!,
                PropertyType = item.PropertyType!
            };

            GenerateProperty(propertyContext);

            if (propertyContext.Members.Count == 0)
                return;

            var membersSource = string.Join("\n\n    ", propertyContext.Members
                .Select(static m => m.Replace("\n", "\n    ")));
            string source = item.Namespace is not null
                ? $$"""
                    namespace {{item.Namespace}};

                    partial class {{item.ClassName}}
                    {
                        {{membersSource}}
                    }
                    """
                : $$"""
                    partial class {{item.ClassName}}
                    {
                        {{membersSource}}
                    }
                    """;
            spc.AddSource($"{item.ClassName}_{item.PropertyName}.g.cs", source);
        });
    }
}
