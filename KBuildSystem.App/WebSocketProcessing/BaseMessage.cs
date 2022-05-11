using System;
using KBuildSystem.App.WebSocketService;

namespace KBuildSystem.App.WebSocketProcessing
{
    public interface IBaseMessage
    {
        void Process(Server server, WebSocketServerWrapper wrapper);
    }
    public abstract class ProcessableMessage : IBaseMessage
    {
        public abstract void Process(Server server, WebSocketServerWrapper wrapper);
    }
}
