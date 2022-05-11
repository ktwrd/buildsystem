using System;
namespace KBuildSystem.App.WebSocketService
{
    public static class WSBuilder
    {
        public static WebSocketServerExtension CreateServer(string location = "ws://127.0.0.1:8888", string path = "/")
        {
            var wssv = new WebSocketServerExtension(location);

            return wssv;
        }
    }
}
