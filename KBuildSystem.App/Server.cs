using System;
using System.Collections.Generic;
using System.Threading;
using KBuildSystem.App.WebSocketService;

namespace KBuildSystem.App
{
    public class Server
    {
        List<Thread> ThreadList = new List<Thread>();
        List<WaitHandle> WaitHandleList = new List<WaitHandle>();

        Dictionary<string, Type> WebSocketProcessingMatch = new Dictionary<string, Type>()
        {
            {@"BuildStatusMessage", typeof(WebSocketProcessing.BuildStatusMessage)}
        };

        public void InitalizeServer()
        {
            EventWaitHandle waithandleWebSocketServer = new EventWaitHandle(false, EventResetMode.ManualReset);
            Thread threadWebSocketServer = new Thread(new ThreadStart(() => Thread_WebSocketServer(waithandleWebSocketServer)));
            WaitHandleList.Add(waithandleWebSocketServer);
            ThreadList.Add(threadWebSocketServer);
        }

        public void StartThreads()
        {
            foreach (Thread thread in ThreadList)
            {
                thread.Start();
            }

            WaitHandle.WaitAll(WaitHandleList.ToArray());
        }

        public string WebSocketServerAddress = @"ws://0.0.0.0:8090";

        public void Thread_WebSocketServer(EventWaitHandle handle)
        {
            var websocketServer = WSBuilder.CreateServer(WebSocketServerAddress);

            websocketServer.AddWebSocketService<WebSocket.Root>(@"/");
            websocketServer.Realm = String.Format(@"KBuildSystem.App");
            websocketServer.Start();

            Console.WriteLine(@"Websocket Server started at: " + WebSocketServerAddress);

            handle.Set();
        }

        public void GetBuildStatus(string buildID)
        {
            Console.WriteLine(String.Format(@"[Server->GetBuildStatus] BuildID: {0}", buildID));
        }
    }
}
