using Flip.Web.Services;
using Flip.Web.Services.Implement;
#if NETCOREAPP
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
#else
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
                .AddSingleton<IFlipService, FlipService>();
        }
    }
#else
    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    public class FlipWebComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.Register<IFlipService, FlipService>();
        }
    }
#endif
}

