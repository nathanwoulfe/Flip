namespace Flip.Models;

public sealed class DocumentTypePropertyResponseModel
{
    public string? Label { get; set; }

    public string? Alias { get; set; }

    public string? NewAlias { get; set; }

    public string? Editor { get; set; }

    public Guid DataTypeKey { get; set; }
}
