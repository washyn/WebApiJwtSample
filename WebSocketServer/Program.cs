using System;
using System.Linq;
using System.Threading;
using WebSocketSharp;
using WebSocketSharp.Server;
namespace WebSocketServer
{

    public class Chat : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            // Console.WriteLine("SecWebSocketKey");
            // Console.WriteLine(this.Context.SecWebSocketKey);
            // // this.Send("Mensaje recibido" + e.Data);
            // // debug object f
            // Console.WriteLine(this.ID); // use this for send...
            // TODO: preguntar a chat gpt...
            // Sessions.Sessions.FirstOrDefault().Context.WebSocket.Send("sdsdadsa");
        }

        protected override void OnError(ErrorEventArgs e)
        {
            
        }

        protected override void OnClose(CloseEventArgs e)
        {
            
        }

        protected override void OnOpen()
        {
            base.OnOpen();
            //
        }
    }
    
    internal class Program
    {
        public static void Main(string[] args)
        {
            var webSocketServer = new WebSocketSharp.Server.WebSocketServer("ws://localhost:8080/");
            webSocketServer.AddWebSocketService<Chat>("/chat");
            webSocketServer.Start();
            
                
            Console.WriteLine("Server started");
            Thread.Sleep(Timeout.Infinite);
        }
    }
}