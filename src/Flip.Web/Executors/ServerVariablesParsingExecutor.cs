using Flip.Web.Controllers;
using System.Collections.Generic;
#if NETCOREAPP
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core;
#else
using Umbraco.Core;
using Umbraco.Core.Services;
#endif

namespace Flip.Web.Executors
{
    public interface IServerVariablesParsingExecutor
    {
        void Generate(IDictionary<string, object> dictionary);
    }

    public class ServerVariablesParsingExecutor : IServerVariablesParsingExecutor
    {
        private readonly IRuntimeState _runtimeState;
        private readonly ILinkGenerator _linkGenerator;

        public ServerVariablesParsingExecutor(IRuntimeState runtimeState, ILinkGenerator linkGenerator)
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

            var flipDictionary = new Dictionary<string, object>
            {
                { "pluginPath", pluginPath },
                { "apiBaseUrl", _linkGenerator.GetUmbracoApiServiceBaseUrl<ApiController>(x => x.GetPermittedTypes(0))},
            };

            dictionary.Add("Flip", flipDictionary);
        }
    }
}