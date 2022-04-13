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
                .AddSingleton<IContentTreeMenuRenderingExecutor, ContentTreeMenuRenderingExecutor>();

            builder.AddNotificationHandler<MenuRenderingNotification, ContentTreeMenuRenderingHandler>();
        }
    }
#else
    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    public class FlipWebComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.Register<IFlipService, FlipService>();
            composition.Register<IContentTreeMenuRenderingExecutor, ContentTreeMenuRenderingExecutor>();

            composition.Components().Append<ContentTreeMenuRenderingComponent>();
        }
    }
#endif
}

