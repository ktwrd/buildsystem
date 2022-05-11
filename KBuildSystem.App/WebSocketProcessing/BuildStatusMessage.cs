using System;
using KBuildSystem.App.WebSocketService;

namespace KBuildSystem.App.WebSocketProcessing
{
    public class BuildStatusMessage : ProcessableMessage
    {
        public override void Process(Server server, WebSocketServerWrapper wrapper)
        {
            server.GetBuildStatus(id);
            wrapper.SendResponse(String.Format(@"The build status for the build with ID of '{0}' would be here!", id));
        }

        public string id { get; set; }
    }
}
