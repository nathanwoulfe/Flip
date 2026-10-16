namespace Flip.Models;

public enum FlipOperationStatus
{
    Unknown,
    Success,
    UserNotFound,
    ContentNotFound,
    ContentTypeNotFound,
    CancelledByNotification,
    CurrentAndNewTypesMatch,
}
