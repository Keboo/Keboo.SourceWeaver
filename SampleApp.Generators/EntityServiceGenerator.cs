using Keboo.SourceWeaver.Sdk.Generators;
using Keboo.SourceWeaver.Sdk.Output;
using Keboo.SourceWeaver.Sdk.Types;

using Microsoft.CodeAnalysis;

namespace SampleApp.Generators;

public class EntityServiceGenerator : AssemblyTypeGenerator
{
    public override GenerationResult Generate(AssemblyTypeContext context)
    {
        TypeDefinition? listType = GetTypeDefinition("System.Collections.Generic.List`1");
        if (listType is null) return GenerationResult.Skip;

        foreach (TypeDefinition type in context.AssemblyTypes)
        {
            if (type.Name == "DbContext")
            {
                foreach (var entityType in type.Properties
                    .Where(x => x.PropertyType == listType)
                    .Select(x => x.GenericTypeArguments[0]))
                {
                    GenerationOutput output = context.CreateFromCurrent();

                    GenerateService(output, entityType);
                }
            }
        }

        return GenerationResult.Success;
    }

    private static void GenerateService(GenerationOutput output, TypeDefinition entityType)
    {
        string entityName = entityType.Name;
        string serviceName = $"{entityName}Service";
        output.AddNamespaceMember($$"""
            public class {{serviceName}}
            {
                private readonly DbContext _context;
                public {{serviceName}}(DbContext context)
                {
                    _context = context;
                }

                public List<{{entityName}}> Get()
                {
                    return _context.{{entityName}}s.ToList();
                }

                public {{entityName}} Get(int id)
                {
                    var entity = _context.{{entityName}}s.Find(id);
                    if (entity == null)
                    {
                        return null;
                    }
                    return entity;
                }
            }
            """);
    }
}
