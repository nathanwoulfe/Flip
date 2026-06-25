using System.Text.Json.Serialization;

namespace Flip.Models;

public class ContentTypeModel
{
    public string? Name { get; set; }

    public Guid Unique { get; set; }

    [JsonPropertyName("defaultTemplateUnique")]
    public Guid? DefaultTemplateKey { get; set; }

    public IEnumerable<PropertyTypeModel> PropertyTypes { get; set; } = [];

    public IEnumerable<TemplateModel> AllowedTemplates { get; set; } = [];
}

public class PropertyTypeModel
{
    public string? Name { get; set; }

    public string? Alias { get; set; }

    [JsonPropertyName("dataTypeUnique")]
    public Guid DataTypeKey { get; set; }

    public string? PropertyEditorAlias { get; set; }
}

public class TemplateModel
{
    public string? Name { get; set; }

    [JsonPropertyName("unique")]
    public Guid Key { get; set; }
}
