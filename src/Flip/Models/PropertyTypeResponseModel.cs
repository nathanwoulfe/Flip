namespace Flip.Models;

public sealed class PropertyTypeResponseModel
{
    public string? Name { get; set; }
    public string? Alias { get; set; }
    public Guid DataTypeKey { get; set; }
    public string? PropertyEditorAlias { get; set; }
}
