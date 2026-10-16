using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Notifications;

namespace Flip.Notifications;

/// <summary>
/// Provides data for an event raised when an element's content type is changing and the operation supports cancellation.
/// </summary>
public class ElementTypeChangingNotification(IElement eventObject, EventMessages messages) : CancelableObjectNotification<IElement>(eventObject, messages)
{
    /// <summary>
    /// Gets the element whose content type is changing.
    /// </summary>
    public IElement Element => Target;
}
