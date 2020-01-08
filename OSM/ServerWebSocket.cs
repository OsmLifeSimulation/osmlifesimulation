using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using OSMLSGlobalLibrary.Map;
using GeoJSON.Net.Feature;
using GeoJSON.Net.CoordinateReferenceSystem;

namespace OSM
{
    public class Client
    {
        public WebSocket Socket { get; private set; }

        public Client(WebSocket socket)
        {
            Socket = socket;
        }

        public async void SendMessageAsync(string message)
        {
            try
            {
                await Socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(message)),
                    WebSocketMessageType.Text, true, CancellationToken.None);
            }
            catch (Exception e)
            {
                //Console.WriteLine("Exception with message send");
            }
        }
    }

    static class ServerWebSocket
    {
        static List<Client> Clients = new List<Client>();
        static HttpListener httpListener = new HttpListener();

        static ModulesLibrary ModulesLibrary { get; set; }

        static MapObjectsCollection MapObjects { get; set; }

        static Timer UpdateTimer;

        public static void Init(ModulesLibrary modulesLibrary, MapObjectsCollection mapObjects)
        {
            ModulesLibrary = modulesLibrary;
            MapObjects = mapObjects;

            httpListener.Prefixes.Add("http://localhost:" + Settings.Presets.WebSocketServerPort + '/');
            httpListener.Start();

            var startTimeSpan = TimeSpan.Zero;
            var periodTimeSpan = TimeSpan.FromSeconds(1);
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
            var featuresCollection =
                MapObjects
                .GetTypeItems()
                .Select(ti =>
                    (ti.type.ToString(),
                    new FeatureCollection(ti.mapObjects.Select(mo => new Feature(mo.Geometry)).ToList()) { CRS = new NamedCRS("EPSG:3857") },
                    ((MapObjectAttribute)ti.type.GetCustomAttributes(typeof(MapObjectAttribute), false).First()).Style)
                )
                .Where(x => x.ToTuple().Item3 != null);

            var jsonData = JsonConvert.SerializeObject(featuresCollection);

            foreach (var client in Clients.ToList())
            {
                if (client.Socket.State != WebSocketState.Open)
                {
                    Clients.Remove(client);
                    Console.WriteLine("Client disconnected");
                }
                client.SendMessageAsync(jsonData);
            }
        }

        public static void AddClient(WebSocket clientSocket)
        {
            var client = new Client(clientSocket);
            Clients.Add(client);

            Console.WriteLine("Client connected");
        }
    }
}
