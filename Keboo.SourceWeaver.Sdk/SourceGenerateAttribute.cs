namespace Keboo.SourceWeaver.Sdk;

public struct GenerationResult
{
    public static GenerationResult Success { get; } = new GenerationResult { IsSuccess = true };
    public static GenerationResult Skip { get; } = new GenerationResult { IsSuccess = false };
    public bool IsSuccess { get; private set; }
}
