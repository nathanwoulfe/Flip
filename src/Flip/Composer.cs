using Flip.Executors;
using Flip.Services;
using Flip.Services.Implement;
using Flip.Handlers;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;


namespace Flip;

internal class Composer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.ManifestFilters().Append<ManifestFilter>();

        builder.Services
            .AddSingleton<IFlipService, FlipService>()
            .AddSingleton<IContentTreeMenuRenderingExecutor, ContentTreeMenuRenderingExecutor>()
            .AddSingleton<IServerVariablesParsingExecutor, ServerVariablesParsingExecutor>();

        builder.AddNotificationHandler<MenuRenderingNotification, ContentTreeMenuRenderingHandler>()
            .AddNotificationHandler<ServerVariablesParsingNotification, ServerVariablesParsingHandler>();
    }
}