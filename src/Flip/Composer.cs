using Flip.Api.Configuration;
using Flip.Services;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Api.Common.OpenApi;
using Umbraco.Cms.Api.Management.OpenApi;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Models;

namespace Flip;

internal class Composer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        _ = builder.Services
            .AddSingleton<IFlipService<IElement>, FlipElementService>()
            .AddSingleton<IFlipService<IContent>, FlipDocumentService>()
            .AddSingleton<IFlipServiceFactory, FlipServiceFactory>();

        _ = builder.AddBackOfficeOpenApiDocument(ApiConstants.ApiName, builder => builder
                .WithTitle(ApiConstants.ApiTitle)
                .WithBackOfficeAuthentication());
    }
}
