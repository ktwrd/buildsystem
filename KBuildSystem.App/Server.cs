using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using KBuildSystem.App.Build;
using KBuildSystem.App.Configuration;
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

            EventWaitHandle waithandleBuildController = new EventWaitHandle(false, EventResetMode.ManualReset);
            Thread threadBuildController = new Thread(new ThreadStart(() => Thread_BuildController(waithandleBuildController)));
            WaitHandleList.Add(waithandleBuildController);
            ThreadList.Add(threadBuildController);
        }

        public void StartThreads()
        {
            foreach (Thread thread in ThreadList)
            {
                thread.Start();
            }

            WaitHandle.WaitAll(WaitHandleList.ToArray());
        }

        public BuildController BuildController;

        public void Thread_BuildController(EventWaitHandle handle)
        {
            BuildController = new BuildController(this, new BuildControllerOptions
            {
                BasePath = ConfigManager.sysRootDataLocation
            });
            handle.Set();
        }

        public string WebSocketServerAddress = String.Format(@"ws://{0}:{1}", ConfigManager.svwsAddress, ConfigManager.svwsPort);

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
