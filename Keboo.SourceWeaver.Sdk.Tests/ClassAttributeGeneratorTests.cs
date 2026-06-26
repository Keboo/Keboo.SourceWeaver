namespace Keboo.SourceWeaver.Sdk.Tests;

using static Keboo.SourceWeaver.Sdk.Tests.ClassAttributeGeneratorTests;


public class ClassAttributeGeneratorTests : SourceGeneratorTestBase<ClassTestGenerator>
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class ClassTestAttribute : Attribute;

    public class ClassTestGenerator : ClassAttributeGenerator<ClassTestAttribute>
    {
        public override GenerationResult Generate(GenerationClassContext context)
        {
            var output = context.FromCurrent();

            output.AddUsing("using System.ComponentModel;");
            output.AddNamespaceMember($$"""
            partial class {{context.ClassName}} : INotifyPropertyChanged
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
