using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using NetTopologySuite.Features;
using NetTopologySuite.IO;
using Newtonsoft.Json;
using OSMLSGlobalLibrary.Map;

namespace OSMLS.Core
{
    internal class WebSocketServer
    {
        internal class Client
        {
            public WebSocket Socket { get; }

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
                catch (Exception)
                {
                    // Do nothing.
                }
            }
        }

		internal struct Package
		{
			public string TypeFullName { get; set; }
			public string FeaturesGeoJson { get; set; }
			public string OpenLayersStyle { get; set; }

			public Package(string typeFullName, string featuresGeoJson, string openLayersStyle)
			{
				TypeFullName = typeFullName;
				FeaturesGeoJson = featuresGeoJson;
				OpenLayersStyle = openLayersStyle;
			}
		}

		private GeoJsonWriter GeoJsonWriter { get; } = new GeoJsonWriter();

        private List<Client> Clients { get; } = new List<Client>();

        private HttpListener HttpListener { get; } = new HttpListener();

        private MapObjectsCollection MapObjects { get; }

        public WebSocketServer(string uri, MapObjectsCollection mapObjects)
        {
            MapObjects = mapObjects;

            HttpListener.Prefixes.Add(uri);
            HttpListener.Start();

            HttpListener.BeginGetContext(OnContext, null);
        }

        private async void OnContext(IAsyncResult ar)
        {
            var context = HttpListener.EndGetContext(ar);
            HttpListener.BeginGetContext(OnContext, null);

            if (context.Request.IsWebSocketRequest)
            {
                var webSocketContext = await context.AcceptWebSocketAsync(null);

                var webSocket = webSocketContext.WebSocket;

                AddClient(webSocket);
            }
        }

        private void AddClient(WebSocket clientSocket)
        {
            var client = new Client(clientSocket);
            Clients.Add(client);

            Console.WriteLine("Client connected");
        }

        public void Update()
        {
            var featuresCollection =
                MapObjects
                .GetTypeItems()
                .Select(ti => new Package
                    (ti.type.FullName,
                    "{\"type\":\"FeatureCollection\", \"features\":" + GeoJsonWriter.Write(new FeatureCollection().Concat(ti.mapObjects.Select(mo => new Feature(mo, new AttributesTable())).ToList())) + "}",
                    ((CustomStyleAttribute)ti.type.GetCustomAttributes(typeof(CustomStyleAttribute), false).FirstOrDefault() ?? new CustomStyleAttribute()).Style)
                )
                .Where(x => x.OpenLayersStyle != null);

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
    }
}
