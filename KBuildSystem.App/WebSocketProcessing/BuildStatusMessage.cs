using System;
using KBuildSystem.App.WebSocketService;

namespace KBuildSystem.App.WebSocketProcessing
{
    public class BuildStatusMessage : ProcessableMessage
    {
        public override void Process(Server server, WebSocketServerWrapper wrapper)
        {
            var build = server.BuildController.GetByID(id);
            wrapper.SendResponse(build.CurrentStatus);
            Console.WriteLine(build == null);
        }

        public string id { get; set; }
    }
}
