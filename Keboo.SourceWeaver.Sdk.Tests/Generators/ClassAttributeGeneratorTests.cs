namespace Keboo.SourceWeaver.Sdk.Tests.Generators;

using Keboo.SourceWeaver.Sdk.Generators;
using Keboo.SourceWeaver.Sdk.Types;

using static Keboo.SourceWeaver.Sdk.Tests.Generators.ClassAttributeGeneratorTests;


public class ClassAttributeGeneratorTests : SourceGeneratorTestBase<ClassTestGenerator>
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class ClassTestAttribute : Attribute;

    public class ClassTestGenerator : ClassAttributeGenerator<ClassTestAttribute>
    {
        public override GenerationResult Generate(GenerationTypeContext context)
        {
            var output = context.CreateFromCurrent();

            output.AddUsing("using System.ComponentModel;");
            output.AddNamespaceMember($$"""
            partial class {{context.Type.Name}} : INotifyPropertyChanged
            {
                public event PropertyChangedEventHandler PropertyChanged;
            }
            """);

            return GenerationResult.Success;
        }
    }

    [Test]
    public async Task WhenClassAttributeIsApplied_ItGeneratesPartialClassImplementation()
    {
        await TestGenerator("""
            using static Keboo.SourceWeaver.Sdk.Tests.ClassAttributeGeneratorTests;

            namespace Test;

            [ClassTest]
            internal partial class Foo 
            {
            }
            """,
            """
            using System.ComponentModel;
            namespace Test
            {
                partial class Foo : INotifyPropertyChanged
                {
                    public event PropertyChangedEventHandler PropertyChanged;
                }
            }

            """,
            "Foo_ClassTestAttribute"
            );
    }
}
