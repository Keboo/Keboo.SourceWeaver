using Keboo.SourceWeaver.Sdk.Types;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Keboo.SourceWeaver.Sdk.Tests.Types;

public class TypeDefintionTests
{
    [Test]
    public async Task TypeDefinition_NamesForInt_ItHasCorrectNameAndFullName()
    {
        var compilation = CSharpCompilation.Create("Test")
            .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));
        INamedTypeSymbol? intType = compilation.GetTypeByMetadataName("System.Int32");

        await Assert.That(intType).IsNotNull();

        var typeDefinition = new TypeDefinition(intType);

        await Assert.That(typeDefinition.Name).IsEqualTo("Int32");
        await Assert.That(typeDefinition.FullName).IsEqualTo("int");
    }

    [Test]
    public async Task TypeDefinition_NamesForList_ItHasCorrectNameAndFullName()
    {
        var compilation = CSharpCompilation.Create("Test")
            .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));
        INamedTypeSymbol? listType = compilation.GetTypeByMetadataName("System.Collections.Generic.List`1");

        await Assert.That(listType).IsNotNull();

        var typeDefinition = new TypeDefinition(listType);

        await Assert.That(typeDefinition.Name).IsEqualTo("List");
        await Assert.That(typeDefinition.FullName).IsEqualTo("global::System.Collections.Generic.List<T>");
    }
}
