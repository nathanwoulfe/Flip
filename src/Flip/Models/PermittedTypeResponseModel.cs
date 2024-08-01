namespace Flip.Models;

public sealed class PermittedTypeResponseModel
{
    public required string Name { get; set; }

    public string? Icon { get; set; }

    public required Guid Unique { get; set; }

    public required string Alias { get; set; }

    public int DefaultTemplateId { get; set; }

    public IEnumerable<TemplateResponseModel> AllowedTemplates { get; set; } = [];

    public IEnumerable<PropertyTypeResponseModel> PropertyTypes { get; set; } = [];
}
