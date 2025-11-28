using Asp.Versioning;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Options;

namespace Flip.Api.Configuration;

internal class OperationIdHandler : Umbraco.Cms.Api.Common.OpenApi.OperationIdHandler
{
    public OperationIdHandler(IOptions<ApiVersioningOptions> apiVersioningOptions) : base(apiVersioningOptions)
    {
    }

    protected override bool CanHandle(ApiDescription apiDescription, ControllerActionDescriptor controllerActionDescriptor)
        => controllerActionDescriptor.ControllerTypeInfo.Namespace?.StartsWith(ApiConstants.ApiNamespacePrefix) is true;
}
