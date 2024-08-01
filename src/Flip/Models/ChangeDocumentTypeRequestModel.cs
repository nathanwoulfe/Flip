namespace Flip.Models;

public sealed class ChangeDocumentTypeRequestModel
{
    public Guid Unique { get; set; }

    public Guid DocumentTypeUnique { get; set; }

    public int? TemplateId { get; set; }

    public IEnumerable<DocumentTypePropertyResponseModel> Properties { get; set; } = [];
}
