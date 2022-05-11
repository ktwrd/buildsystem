using System;
using WebSocketSharp;

namespace KBuildSystem.App.WebSocketService
{
    public class WebSocketMessageEventArgs : EventArgs
    {
        public string Data { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public MessageEventArgs Message { get; set; }
    }
}
