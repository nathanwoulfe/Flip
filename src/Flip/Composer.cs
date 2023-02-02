using Flip.Executors;
using Flip.Handlers;
using Flip.Services;
using Flip.Services.Implement;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;

namespace Flip;

internal class Composer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        _ = builder.ManifestFilters().Append<ManifestFilter>();

        _ = builder.Services
            .AddSingleton<IFlipService, FlipService>()
            .AddSingleton<IContentTreeMenuRenderingExecutor, ContentTreeMenuRenderingExecutor>()
            .AddSingleton<IServerVariablesParsingExecutor, ServerVariablesParsingExecutor>();

        _ = builder.AddNotificationHandler<MenuRenderingNotification, ContentTreeMenuRenderingHandler>()
            .AddNotificationHandler<ServerVariablesParsingNotification, ServerVariablesParsingHandler>();
    }
}
