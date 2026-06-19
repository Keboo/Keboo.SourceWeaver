namespace Keboo.SourceWeaver.Sdk.Tests;

public class IndentingStringBuilderTests
{
    [Test]
    public async Task IndentAndOutdent_ShouldIncreaseAndDecreaseIndentCount()
    {
        var builder = new IndentingStringBuilder();
        await Assert.That(builder.IndentCount).IsEqualTo(0);
        builder.Indent();
        await Assert.That(builder.IndentCount).IsEqualTo(1);
        builder.Indent();
        await Assert.That(builder.IndentCount).IsEqualTo(2);
        builder.Outdent();
        await Assert.That(builder.IndentCount).IsEqualTo(1);
        builder.Outdent();
        await Assert.That(builder.IndentCount).IsEqualTo(0);
    }
}
