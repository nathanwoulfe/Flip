using Umbraco.Cms.Api.Common.OpenApi;

namespace Umbraco.Workflow.Web.Api.Configuration;

internal sealed class FlipSchemaIdHandler : SchemaIdHandler
{
    public override bool CanHandle(Type type)
        => type.Namespace?.StartsWith("Flip.") is true;
}
