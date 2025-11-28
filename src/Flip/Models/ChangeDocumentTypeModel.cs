using System.Text.Json.Serialization;

namespace Flip.Models;

public sealed class ChangeDocumentTypeModel
{
    public Guid Unique { get; set; }

    [JsonPropertyName("contentTypeUnique")]
    public Guid ContentTypeKey { get; set; }

    public int? TemplateId { get; set; }

    public IEnumerable<DocumentTypePropertyModel>? Properties { get; set; }
}
