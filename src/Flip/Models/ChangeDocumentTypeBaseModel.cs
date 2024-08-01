namespace Flip.Models;

public class ChangeDocumentTypeBaseModel
{
    public Guid Unique { get; set; }

    public DocumentTypeResponseModel DocumentType { get; set; } = new DocumentTypeResponseModel { Alias = string.Empty, Name = string.Empty };

    public int? TemplateId { get; set; }

    public IEnumerable<DocumentTypePropertyResponseModel> Properties { get; set; } = [];
}
