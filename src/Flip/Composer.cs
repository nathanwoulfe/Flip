using Flip.Api.Configuration;
using Flip.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using Umbraco.Cms.Api.Common.OpenApi;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace Flip;

internal class Composer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        _ = builder.Services
            .AddSingleton<IFlipService, FlipService>();

        builder.Services.Configure<SwaggerGenOptions>(options =>
        {
            options.SwaggerDoc(
                ApiConstants.ApiName,
                new OpenApiInfo
                {
                    Title = ApiConstants.ApiTitle,
                    Version = "Latest",
                    Description = $"Describes the {ApiConstants.ApiTitle} available for the Flip backoffice extension."
                });

            options.OperationFilter<BackOfficeSecurityRequirementsOperationFilter>();
        }).AddSingleton<IOperationIdHandler, Api.Configuration.OperationIdHandler>();
    }
}
