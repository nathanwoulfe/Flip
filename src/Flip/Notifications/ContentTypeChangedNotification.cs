using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Notifications;

namespace Flip.Notifications;

/// <summary>
/// Provides data for an event raised when a document's content type has changed.
/// </summary>
public class ContentTypeChangedNotification(IContent eventObject, EventMessages messages) : ObjectNotification<IContent>(eventObject, messages)
{
    /// <summary>
    /// Gets the document whose content type has changed.
    /// </summary>
    public IContent Content => Target;
}
