using Flip.Web.Executors;
using Flip.Web.Services;
using Flip.Web.Services.Implement;
#if NETCOREAPP
using Flip.Web.Handlers;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;
#else
using Flip.Web.Components;
using Umbraco.Core;
using Umbraco.Core.Composing;
#endif

namespace Flip.Web
{
#if NETCOREAPP
    public class FlipWebComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services
                .AddSingleton<IFlipService, FlipService>()
                .AddSingleton<ILinkGenerator, LinkGenerator>()
                .AddSingleton<IContentTreeMenuRenderingExecutor, ContentTreeMenuRenderingExecutor>()
                .AddSingleton<IServerVariablesParsingExecutor, ServerVariablesParsingExecutor>();

            builder.AddNotificationHandler<MenuRenderingNotification, ContentTreeMenuRenderingHandler>()
                .AddNotificationHandler<ServerVariablesParsingNotification, ServerVariablesParsingHandler>();
        }
    }
#else
    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    public class FlipWebComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.Register<IFlipService, FlipService>();
            composition.Register<ILinkGenerator, LinkGenerator>();
            composition.Register<IContentTreeMenuRenderingExecutor, ContentTreeMenuRenderingExecutor>();
            composition.Register<IServerVariablesParsingExecutor, ServerVariablesParsingExecutor>();

            composition.Components()
                .Append<ContentTreeMenuRenderingComponent>()
                .Append<ServerVariablesParsingComponent>();
        }
    }
#endif
}

