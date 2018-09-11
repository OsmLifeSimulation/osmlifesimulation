using OSM.Simulated_Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OSM
{
    static class ServerWebSocket
    {
        static List<WebSocket> Clients = new List<WebSocket>();
        static HttpListener httpListener = new HttpListener();

        public static List<Character> Characters { get; set; }


        static Timer UpdateTimer;

        public static void Init(List<Character> characters)
        {
            Characters = characters;

            httpListener.Prefixes.Add("http://localhost:8080/");
            httpListener.Start();

            var startTimeSpan = TimeSpan.Zero;
            var periodTimeSpan = TimeSpan.FromMilliseconds(150);
            UpdateTimer = new Timer((e) =>
            {
                Update();
            }, null, startTimeSpan, periodTimeSpan);

            httpListener.BeginGetContext(OnContext, null);
        }

        private static async void OnContext(IAsyncResult ar)
        {
            HttpListenerContext context = httpListener.EndGetContext(ar);
            httpListener.BeginGetContext(OnContext, null);

            if (context.Request.IsWebSocketRequest)
            {
                HttpListenerWebSocketContext webSocketContext = await context.AcceptWebSocketAsync(null);

                WebSocket webSocket = webSocketContext.WebSocket;
                AddClient(webSocket);
            }
        }

        private static void Update()
        {
            var coordinates = Characters.ToList().Select(c => MathExtensions.UTM2Deg(c.Point.ToVector2()))
                .Select(v => new[] { v.Y, v.X }).ToList();
            var jsonData = JsonConvert.SerializeObject(coordinates);

            foreach (var client in Clients.ToList())
            {
                if (client.State != WebSocketState.Open)
                {
                    Clients.Remove(client);
                    Console.WriteLine("Client disconnected");

                }
                client.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(jsonData)),
                    WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        public static void AddClient(WebSocket clientSocket)
        {
            Clients.Add(clientSocket);
        }
    }
}
