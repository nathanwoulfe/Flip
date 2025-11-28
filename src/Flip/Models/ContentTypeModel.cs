using Umbraco.Cms.Api.Management.Controllers.User.ClientCredentials;
using Umbraco.Cms.Api.Management.ViewModels.Template;

namespace Flip.Models;

public class ContentTypeModel
{
    public string? Name { get; set; }

    public Guid Unique { get; set; }

    public int DefaultTemplateId { get; set; }

    public IEnumerable<PropertyTypeModel> PropertyTypes { get; set; } = [];

    public IEnumerable<TemplateModel> AllowedTemplates { get; set; } = [];
}

public class PropertyTypeModel
{
    public string? Name { get; set; }

    public string? Alias { get; set; }

    public Guid DataTypeKey { get; set; }

    public string? PropertyEditorAlias { get; set; }
}

public class TemplateModel
{
    public string? Name { get; set; }
    public int Id { get; set; }
}
