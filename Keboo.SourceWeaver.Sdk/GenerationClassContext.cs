namespace Keboo.SourceWeaver.Sdk;


public class GenerationClassContext : GenerationContext
{
    public string ClassName { get; init; } = "";

    public override GenerationOutput FromCurrent()
    {
        var rv = base.FromCurrent();
        rv.ClassName = ClassName;
        return rv;
    }
}
