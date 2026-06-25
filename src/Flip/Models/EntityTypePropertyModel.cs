using System.Text.Json.Serialization;

namespace Flip.Models;

public sealed class EntityTypePropertyModel
{
    public string? Label { get; set; }

    public string? Alias { get; set; }

    [JsonIgnore]
    public object? Value { get; set; }

    [JsonIgnore]
    public IEnumerable<(string? Culture, object? Value)>? Values { get; set; }

    public string? NewAlias { get; set; }

    public string? Editor { get; set; }

    [JsonPropertyName("dataTypeUnique")]
    public Guid? DataTypeKey { get; set; }
}
