using Flip.Controllers;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core;
using Microsoft.AspNetCore.Routing;
using Umbraco.Extensions;

namespace Flip.Executors;

public interface IServerVariablesParsingExecutor
{
    void Generate(IDictionary<string, object> dictionary);
}

internal sealed class ServerVariablesParsingExecutor : IServerVariablesParsingExecutor
{
    private readonly IRuntimeState _runtimeState;
    private readonly LinkGenerator _linkGenerator;

    public ServerVariablesParsingExecutor(IRuntimeState runtimeState, LinkGenerator linkGenerator)
    {
        _runtimeState = runtimeState;
        _linkGenerator = linkGenerator;
    }

    public void Generate(IDictionary<string, object> dictionary)
    {
        if (_runtimeState.Level != RuntimeLevel.Run)
            return;

        Dictionary<string, object> umbracoSettings = dictionary["umbracoSettings"] as Dictionary<string, object> ?? new Dictionary<string, object>();
        string pluginPath = $"{umbracoSettings["appPluginsPath"]}/Flip/Backoffice";

        Dictionary<string, object> flipDictionary = new()
        {
            { "pluginPath", pluginPath },
            { "apiBaseUrl", _linkGenerator.GetUmbracoApiServiceBaseUrl<ApiController>(x => x.GetPermittedTypes(0))!},
        };

        dictionary.Add("Flip", flipDictionary);
    }
}