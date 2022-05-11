using System;
using System.Text;
using System.Collections.Generic;
using KBuildSystem.App.WebSocketService;
using Newtonsoft.Json;
using KBuildSystem.App.WebSocketProcessing;
using System.Linq;

namespace KBuildSystem.App.WebSocket
{
    internal class Root : WebSocketServerWrapper
    {
        protected override void OnWebSocketMessage(WebSocketMessageEventArgs e, Server server)
        {
            Sessions.Sweep();
            string[] lines = e.Data.Split(
                new string[] { "\r\n", "\r", "\n" },
                StringSplitOptions.None);

            string typeName = lines[0];
            string rawJSONData = e.Data.Replace(lines[0], "");
            Type jsonType = Type.GetType(typeName);
            if (!typeName.StartsWith(@"KBuildSystem.App.WebSocketProcessing", StringComparison.Ordinal)) return;
            Dictionary<string, Type> dictTypeMap = new Dictionary<string, Type>();
            dictTypeMap.Add(@"BuildStatusMessage", typeof(BuildStatusMessage));
            foreach (KeyValuePair<string, Type> pair in dictTypeMap)
            {
                if (typeName.StartsWith(@"KBuildSystem.App.WebSocketProcessing." + pair.Key, StringComparison.Ordinal))
                {
                    var mi = typeof(JsonConvert)
                        .GetMethods()
                        .Where(x => x.Name == @"DeserializeObject")
                        .FirstOrDefault(x => x.IsGenericMethod);
                    var objRef = mi.MakeGenericMethod(pair.Value);
                    dynamic result = objRef.Invoke(typeof(JsonConvert), new object[] { rawJSONData });
                    result.Process(server, this);
                    break;
                }
            }
        }

        protected override void OnOpen()
        {
            base.OnOpen();
        }
    }
}
