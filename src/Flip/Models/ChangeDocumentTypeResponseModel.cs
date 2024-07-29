namespace Flip.Models;

public sealed class ChangeDocumentTypeResponseModel
{
    public string? NodeName { get; set; }

    public Guid Unique { get; set; }

    public int ContentTypeId { get; set; }

    public string? ContentTypeName { get; set; }

    public int? TemplateId { get; set; }

    public IEnumerable<DocumentTypePropertyResponseModel>? Properties { get; set; }
}
