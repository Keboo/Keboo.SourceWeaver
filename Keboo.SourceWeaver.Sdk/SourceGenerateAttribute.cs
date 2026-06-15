namespace Keboo.SourceWeaver.Sdk;

public sealed class GenerationResult
{
    public static GenerationResult Success { get; } = new GenerationResult { IsSuccess = true };
    public bool IsSuccess { get; private set; }
}
