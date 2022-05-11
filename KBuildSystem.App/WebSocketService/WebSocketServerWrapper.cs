using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using System.Threading.Tasks;
using Newtonsoft.Json;
using WebSocketSharp;
using WebSocketSharp.Server;

#pragma warning disable CS0618 // Type or member is obsolete

namespace KBuildSystem.App.WebSocketService
{
    public class WebSocketServerWrapper : WebSocketBehavior
    {
        public Server server = KBuildSystem.App.MainClass.Server;
        public static byte[] CreateFullByteArray(int length, byte thing)
        {
            var arr = new byte[length];
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = thing;
            }
            return arr;
        }
        public event EventHandler<WebSocketMessageEventArgs> WebSocketMessage;
        protected override void OnMessage(MessageEventArgs e)
        {
            if (e.Type != Opcode.Text) return;

            Console.WriteLine("================ Message Start ================");
            Console.WriteLine(e.Data);
            Console.WriteLine("===============================================");

            DateTime current = DateTime.Now;

            byte[] response = new byte[0];

            WebSocketMessageEventArgs msgargs = new WebSocketMessageEventArgs()
            {
                Data = e.Data,
                Timestamp = (DateTimeOffset)current,
                Message = e
            };
            if (response.Length > 0)
            {
                Send(response);
            }
            else
            {
                OnWebSocketMessage(msgargs, server);
            }
        }
        protected virtual void OnWebSocketMessage(WebSocketMessageEventArgs e, Server server)
        {
            EventHandler<WebSocketMessageEventArgs> handler = WebSocketMessage;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        public void SendResponse(dynamic content)
        {
            byte[] data = GenerateResponse(content);
            Send(data);
        }
        public async Task<bool> SendAsyncResponse(dynamic content)
        {
            byte[] data = GenerateResponse(content);
            var promise = new TaskCompletionSource<bool>();
            bool res = false;
            Action<bool> whenSendIsDone = (bool stat) => {
                promise.SetResult(stat);
                res = stat;
            };
            SendAsync(data, whenSendIsDone);
            await Task.WhenAny(promise.Task);
            return res;
        }
        protected byte[] GenerateResponse(dynamic content)
        {
            List<string> result = new List<string>();

            result.Add(content.GetType().FullName + "\n");
            // Convert the content into a formatted JSON
            result.Add(JsonConvert.SerializeObject(content));


            int length = 0;
            for (int i = 0; i < result.Count; i++)
            {
                Console.WriteLine(result[i]);
                length += Encoding.UTF8.GetByteCount((string)result[i]);
            }
            Console.WriteLine("============  Message Content End  ============");

            byte[] returnableByteArray = new byte[0];
            for (int i = 0; i < result.Count; i++)
            {
                returnableByteArray = returnableByteArray.Concat(Encoding.UTF8.GetBytes((string)result[i])).ToArray();
            }

            return returnableByteArray;
        }
    }
}
