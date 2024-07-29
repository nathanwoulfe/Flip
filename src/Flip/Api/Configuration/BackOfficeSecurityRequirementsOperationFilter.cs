using Umbraco.Cms.Api.Management.OpenApi;

namespace Flip.Api.Configuration;

public class BackOfficeSecurityRequirementsOperationFilter : BackOfficeSecurityRequirementsOperationFilterBase
{
    protected override string ApiName => ApiConstants.ApiName;
}
