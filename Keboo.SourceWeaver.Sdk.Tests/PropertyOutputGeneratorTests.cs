namespace Keboo.SourceWeaver.Sdk.Tests;

using System.Text;

using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Text;

using VerifyCS = CSharpSourceGeneratorVerifier<PropertyOutputGeneratorTests.TestGenerator>;


public class PropertyOutputGeneratorTests : UnitTestBase
{
    public sealed class PropertyTestAttribute : Attribute;

    internal class TestGenerator : PropertyAttributeGenerator<PropertyTestAttribute>
    {
        public override GenerationResult Generate(GenerationPropertyContext context)
        {
            var output = context.FromCurrent();

            output.AddClassMember($$"""
            public partial {{context.PropertyType}} {{context.PropertyName}}
            {
                get => field;
                set
                {
                    if (field != value)
                    {
                        field = value;
                    }
                }
            }
            """);

            return GenerationResult.Success;
        }
    }

    [Test]
    public async Task WhenPropertyAttributeIsApplied_ItGeneratesPartialPropertyImplementation()
    {
        await new VerifyCS.Test
        {
            TestCode = """
            using static Keboo.SourceWeaver.Sdk.Tests.PropertyOutputGeneratorTests;

            namespace Test;

            internal partial class Foo 
            {
                [PropertyTest]
                public partial int Number { get; set; }
            }
            """,

            TestState =
            {
                GeneratedSources =
                {
                    GetSourceFile("""
                    namespace Test
                    {
                        partial class Foo
                        {
                            public partial int Number
                            {
                                get => field;
                                set
                                {
                                    if (field != value)
                                    {
                                        field = value;
                                    }
                                }
                            }
                        }
                    }

                    """, "Foo_Number_PropertyTestAttribute.g.cs")
                }
            }
        }.RunAsync(CT);
    }

    private static (string FileName, SourceText SourceText) GetSourceFile(string content, string fileName)
    {
        fileName = Path.Combine("Keboo.SourceWeaver.Sdk.Tests", "Keboo.SourceWeaver.Sdk.Tests.PropertyOutputGeneratorTests+TestGenerator", fileName);
        return (fileName, SourceText.From(content, Encoding.UTF8, SourceHashAlgorithm.Sha256));
    }
}
