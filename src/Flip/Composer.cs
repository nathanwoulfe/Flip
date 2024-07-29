using Flip.Extensions;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace Flip;

internal class Composer : IComposer
{
    public void Compose(IUmbracoBuilder builder) => builder.AddFlip();
}
