using Newtonsoft.Json;

namespace Flip.Models;

public sealed class ChangeDocumentTypeModel
{
    [JsonProperty("nodeName")]
    public string? NodeName { get; set; }

    [JsonProperty("nodeId")]
    public int NodeId { get; set; }

    [JsonProperty("contentTypeId")]
    public int ContentTypeId { get; set; }

    [JsonProperty("contentTypeName")]
    public string? ContentTypeName { get; set; }

    [JsonProperty("templateId")]
    public int? TemplateId { get; set; }

    [JsonProperty("properties")]
    public IEnumerable<DocumentTypePropertyModel>? Properties { get; set; }
}
