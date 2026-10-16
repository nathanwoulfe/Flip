using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Notifications;

namespace Flip.Notifications;

/// <summary>
/// Provides data for an event raised when a document's content type is changing and the operation supports cancellation.
/// </summary>
public class ContentTypeChangingNotification(IContent eventObject, EventMessages messages) : CancelableObjectNotification<IContent>(eventObject, messages)
{
    /// <summary>
    /// Gets the document whose content type is changing.
    /// </summary>
    public IContent Content => Target;
}
