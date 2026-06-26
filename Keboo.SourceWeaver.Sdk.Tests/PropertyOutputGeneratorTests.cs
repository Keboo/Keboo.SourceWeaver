namespace Keboo.SourceWeaver.Sdk.Tests;

using static Keboo.SourceWeaver.Sdk.Tests.PropertyOutputGeneratorTests;


public class PropertyOutputGeneratorTests : SourceGeneratorTestBase<PropertyTestGenerator>
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class PropertyTestAttribute : Attribute;

    public class PropertyTestGenerator : PropertyAttributeGenerator<PropertyTestAttribute>
    {
        public override GenerationResult Generate(GenerationPropertyContext context)
        {
            var output = context.FromCurrent();

            output.AddClassMember($$"""
            {{context.PropertyAccessModifier:+}}partial {{context.PropertyType}} {{context.PropertyName}}
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
        await TestGenerator("""
            using static Keboo.SourceWeaver.Sdk.Tests.PropertyOutputGeneratorTests;

            namespace Test;

            internal partial class Foo 
            {
                [PropertyTest]
                public partial int Number { get; set; }
            }
            """,
            """
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

            """,
            "Foo_Number_PropertyTestAttribute"
            );
    }

    [Test]
    public async Task WhenPropertyIsPrivate_ItGeneratesPartialPropertyImplementation()
    {
        await TestGenerator("""
            using static Keboo.SourceWeaver.Sdk.Tests.PropertyOutputGeneratorTests;

            namespace Test;

            internal partial class Foo 
            {
                [PropertyTest]
                private partial int Number { get; set; }
            }
            """,
            """
            namespace Test
            {
                partial class Foo
                {
                    private partial int Number
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

            """,
            "Foo_Number_PropertyTestAttribute"
            );
    }

    [Test]
    public async Task WhenPropertyHasNoAccessModifier_ItGeneratesPartialPropertyImplementation()
    {
        await TestGenerator("""
            using static Keboo.SourceWeaver.Sdk.Tests.PropertyOutputGeneratorTests;

            namespace Test;

            partial class Foo 
            {
                [PropertyTest]
                partial int Number { get; set; }
            }
            """,
            """
            namespace Test
            {
                partial class Foo
                {
                    partial int Number
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

            """,
            "Foo_Number_PropertyTestAttribute"
            );
    }
}
