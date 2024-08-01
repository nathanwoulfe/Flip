namespace Flip.Models;

public sealed class DocumentTypeResponseModel
{
    public required string Name { get; set; }
    public required string Alias { get; set; }
    public Guid Unique { get; set; }
    public string? Icon { get; set; }
}
