using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Notifications;

namespace Flip.Notifications;

/// <summary>
/// Provides data for an event raised when an element's content type has changed.
/// </summary>
public class ElementTypeChangedNotification(IElement eventObject, EventMessages messages) : ObjectNotification<IElement>(eventObject, messages)
{
    /// <summary>
    /// Gets the element whose content type has changed.
    /// </summary>
    public IElement Element => Target;
}
