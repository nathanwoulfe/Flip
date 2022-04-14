#if NETFRAMEWORK
using Flip.Web.Executors;
using System;
using System.Collections.Generic;
using Umbraco.Core.Composing;
using Umbraco.Web.JavaScript;

namespace Flip.Web.Components {

    public class ServerVariablesParsingComponent : IComponent
    {
        private readonly IServerVariablesParsingExecutor _serverVariablesParsingExecutor;

        public ServerVariablesParsingComponent(IServerVariablesParsingExecutor serverVariablesParsingExecutor) =>         
            _serverVariablesParsingExecutor = serverVariablesParsingExecutor;

        public void Initialize() => ServerVariablesParser.Parsing += ServerVariablesParser_Parsing;     
        
        public void Terminate() => ServerVariablesParser.Parsing -= ServerVariablesParser_Parsing;    

        private void ServerVariablesParser_Parsing(object sender, IDictionary<string, object> e) =>
            _serverVariablesParsingExecutor.Generate(e);          
    }
}
#endif