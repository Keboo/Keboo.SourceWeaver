using System.Collections.Immutable;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;

namespace Keboo.SourceWeaver.Sdk.Tests;

public static class CSharpSourceGeneratorVerifier<TSourceGenerator>
    where TSourceGenerator : IIncrementalGenerator, new()
{
    public class Test : CSharpSourceGeneratorTest<EmptySourceGeneratorProvider, DefaultVerifier>
    {
        public Test()
        {
            ReferenceAssemblies = ReferenceAssemblies.Net.Net100;
        }

        public void SetGlobalOption(string key, string value)
        {
            // Create or update .editorconfig content
            var existingConfig = TestState.AnalyzerConfigFiles.FirstOrDefault();
            var configBuilder = new StringBuilder();

            if (existingConfig.content != null)
            {
                configBuilder.Append(existingConfig.content);
            }
            else
            {
                configBuilder.AppendLine("is_global = true");
            }

            configBuilder.AppendLine($"{key} = {value}");

            TestState.AnalyzerConfigFiles.Clear();
            TestState.AnalyzerConfigFiles.Add(("/.globalconfig", configBuilder.ToString()));
        }

        protected override IEnumerable<Type> GetSourceGenerators()
        {
            yield return typeof(TSourceGenerator);
        }

        protected override Project ApplyCompilationOptions(Project project)
        {
            string fullPath = typeof(UnitTestBase).Assembly.Location;
            project = project.AddMetadataReference(MetadataReference.CreateFromFile(fullPath));

            return project;
        }

        protected override CompilationOptions CreateCompilationOptions()
        {
            var compilationOptions = base.CreateCompilationOptions();
            return compilationOptions.WithSpecificDiagnosticOptions(
                 compilationOptions.SpecificDiagnosticOptions.SetItems(GetNullableWarningsFromCompiler()));
        }

        public LanguageVersion LanguageVersion { get; set; } = LanguageVersion.Default;

        private static ImmutableDictionary<string, ReportDiagnostic> GetNullableWarningsFromCompiler()
        {
            string[] args = ["/warnaserror:nullable"];
            var commandLineArguments = CSharpCommandLineParser.Default.Parse(args, baseDirectory: Environment.CurrentDirectory, sdkDirectory: Environment.CurrentDirectory);
            var nullableWarnings = commandLineArguments.CompilationOptions.SpecificDiagnosticOptions;

            return nullableWarnings;
        }

        protected override ParseOptions CreateParseOptions()
        {
            return ((CSharpParseOptions)base.CreateParseOptions()).WithLanguageVersion(LanguageVersion);
        }
    }

    internal class TestAnalyzerConfigOptionsProvider : AnalyzerConfigOptionsProvider
    {
        private readonly TestAnalyzerConfigOptions _globalOptions;

        public TestAnalyzerConfigOptionsProvider(Dictionary<string, string> globalOptions)
        {
            _globalOptions = new TestAnalyzerConfigOptions(globalOptions);
        }

        public override AnalyzerConfigOptions GlobalOptions => _globalOptions;

        public override AnalyzerConfigOptions GetOptions(SyntaxTree tree)
            => TestAnalyzerConfigOptions.Empty;

        public override AnalyzerConfigOptions GetOptions(AdditionalText textFile)
            => TestAnalyzerConfigOptions.Empty;
    }

    internal class TestAnalyzerConfigOptions : AnalyzerConfigOptions
    {
        private readonly Dictionary<string, string> _options;

        public static readonly TestAnalyzerConfigOptions Empty = new([]);

        public TestAnalyzerConfigOptions(Dictionary<string, string> options) => _options = options;

        public override bool TryGetValue(string key, out string value)
        {
            return _options.TryGetValue(key, out value!);
        }
    }
}

