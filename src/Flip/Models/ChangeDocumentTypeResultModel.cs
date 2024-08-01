namespace Flip.Models;

public sealed record ChangeDocumentTypeResultModel
{
    public required bool Success { get; set; }
    public required string Message { get; set; }
}
