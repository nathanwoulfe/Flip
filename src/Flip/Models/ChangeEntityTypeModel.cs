using System.Text.Json.Serialization;

namespace Flip.Models;

public sealed class ChangeEntityTypeModel
{
    public Guid Unique { get; set; }

    [JsonPropertyName("contentTypeUnique")]
    public Guid ContentTypeKey { get; set; }

    [JsonPropertyName("templateUnique")]
    public Guid? TemplateKey { get; set; }

    public IEnumerable<EntityTypePropertyModel>? Properties { get; set; }
}
