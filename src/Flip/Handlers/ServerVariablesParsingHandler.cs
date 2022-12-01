using Flip.Executors;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;

namespace Flip.Handlers;

public sealed class ServerVariablesParsingHandler : INotificationHandler<ServerVariablesParsingNotification>
{
    private readonly IServerVariablesParsingExecutor _serverVariablesParsingExecutor;

    public ServerVariablesParsingHandler(IServerVariablesParsingExecutor serverVariablesParsingExecutor) =>
        _serverVariablesParsingExecutor = serverVariablesParsingExecutor;        

    public void Handle(ServerVariablesParsingNotification notification) =>
        _serverVariablesParsingExecutor.Generate(notification.ServerVariables);        
}
