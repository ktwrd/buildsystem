using System;
using System.Linq;
using System.Net;
using System.Reflection;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace WebSocketSharp.Server
{
    public enum ServerState
    {
        Ready,
        Start,
        ShuttingDown,
        Stop
    }
}
namespace KBuildSystem.App.WebSocketService
{
    public class OnStopEventArgs : EventArgs
    {
        public OnStopEventArgs()
        {
        }
        public OnStopEventArgs(ushort code, string reason)
        {
            Code = code;
            Reason = reason;
        }
        public OnStopEventArgs(CloseStatusCode code, string reason)
        {
            Code = (ushort)code;
            Reason = reason;
        }

        public ushort Code;
        public string Reason;
    }
    public class WebSocketServerExtension : WebSocketServer
    {
        public Server Server = null;
        public WebSocketServerExtension()
            : base()
        {
        }
        public WebSocketServerExtension(int port)
            : base(port)
        {
        }
        public WebSocketServerExtension(string url)
            : base(url)
        {
        }
        public WebSocketServerExtension(int port, bool secure)
            : base(port, secure)
        {
        }
        public WebSocketServerExtension(IPAddress address, int port)
            : base(address, port)
        {
        }
        public WebSocketServerExtension(IPAddress address, int port, bool secure)
            : base(address, port, secure)
        {
        }
        public ServerState State
        {
            get
            {
                FieldInfo[] receivedObject = typeof(WebSocketServer).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
                ServerState state = ServerState.Ready;
                for (int i = 0; i < receivedObject.Length; i++)
                {
                    if (receivedObject[i].Name == "_state")
                    {
                        state = (ServerState)receivedObject[i].GetValue(this);
                    }
                }
                return state;
            }
        }

        public event EventHandler<EventArgs> OnStart;
        public new void Start()
        {
            EventHandler<EventArgs> handler = OnStart;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
            Console.WriteLine(@"Starting WebSocket Server");
            base.Start();
        }

        public event EventHandler<OnStopEventArgs> OnStop;
        public new void Stop()
        {
            EventHandler<OnStopEventArgs> handler = OnStop;
            if (handler != null)
            {
                handler(this, new OnStopEventArgs());
                Console.WriteLine(@"ev");
            }
            Console.WriteLine(@"Stopping WebSocket Server");
            base.Stop();
        }
        public new void Stop(ushort code, string reason)
        {
            EventHandler<OnStopEventArgs> handler = OnStop;
            if (handler != null)
            {
                handler(this, new OnStopEventArgs(code, reason));
                Console.WriteLine(@"ev");
            }
            Console.WriteLine(@"Stopping WebSocket Server");
            base.Stop(code, reason);
        }
        public new void Stop(CloseStatusCode code, string reason)
        {
            EventHandler<OnStopEventArgs> handler = OnStop;
            if (handler != null)
            {
                handler(this, new OnStopEventArgs(code, reason));
                Console.WriteLine(@"ev");
            }
            Console.WriteLine(@"Stopping WebSocket Server");
            base.Stop(code, reason);
        }
    }
}
