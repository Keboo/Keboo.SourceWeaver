using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Keboo.SourceWeaver.Sdk.Tests;

public abstract class SourceGeneratorTestBase<TGenerator> : UnitTestBase
    where TGenerator : IIncrementalGenerator, new()
{
    protected async Task TestGenerator(string inputCode, string expectedGeneratedCode, string fileName)
    {
        var test = new CSharpSourceGeneratorVerifier<TGenerator>.Test
        {
            TestCode = inputCode,
            TestState =
            {
                GeneratedSources =
                {
                    GetSourceFile(expectedGeneratedCode, $"{fileName}.g.cs")
                }
            }
        };
        await test.RunAsync(CT);
    }


    protected (string FileName, SourceText SourceText) GetSourceFile(string content, string fileName)
    {
        fileName = Path.Combine(
            "Keboo.SourceWeaver.Sdk.Tests", 
            $"Keboo.SourceWeaver.Sdk.Tests.{GetType().Name}+{typeof(TGenerator).Name}", 
            fileName
        );
        return (fileName, SourceText.From(content, Encoding.UTF8, SourceHashAlgorithm.Sha256));
    }
}