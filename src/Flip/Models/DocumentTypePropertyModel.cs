using Newtonsoft.Json;
using Umbraco.Cms.Core.PropertyEditors;

namespace Flip.Models;

public sealed class DocumentTypePropertyModel
{
    [JsonProperty("label")]
    public string? Label { get; set; }

    [JsonProperty("alias")]
    public string? Alias { get; set; }

    [JsonProperty("value")]
    public object? Value { get; set; }

    [JsonProperty("values")]
    public IEnumerable<(string? Culture, object? Value)>? Values {get; set;} 

    [JsonProperty("newAlias")]
    public string? NewAlias { get; set; }

    [JsonProperty("editor")]
    public string? Editor { get; set; }

    [JsonProperty("dataTypeKey")]
    public string? DataTypeKey { get; set; }
}
