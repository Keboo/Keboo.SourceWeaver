namespace Keboo.SourceWeaver.Sdk.Tests;

public abstract class UnitTestBase
{
    protected static CancellationToken CT => TestContext.Current!.Execution.CancellationToken;
}
