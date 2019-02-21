using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using Microsoft.Xna.Framework;

namespace OSM
{
    public class Client
    {
        public WebSocket Socket { get; private set; }
        public ClientRole Role { get; set; }

        public enum ClientRole { User, Admin }

        public Client(WebSocket socket)
        {
            Socket = socket;
            Role = ClientRole.User;
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

        public static ModulesLibrary ModulesLibrary { get; set; }

        static Timer UpdateTimer;

        public static void Init(ModulesLibrary modulesLibrary)
        {
            ModulesLibrary = modulesLibrary;

            httpListener.Prefixes.Add("http://localhost:" + Settings.Presets.WebSocketServerPort + '/');
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

        static async void HandleClient(object o)
        {
            Client client = (Client)o;

            while (client.Socket.State == WebSocketState.Open)
            {
                ArraySegment<Byte> buffer = new ArraySegment<byte>(new Byte[8192]);

                WebSocketReceiveResult result = null;

                try
                {
                    using (var ms = new MemoryStream())
                    {
                        do
                        {
                            result = await client.Socket.ReceiveAsync(buffer, CancellationToken.None);
                            ms.Write(buffer.Array, buffer.Offset, result.Count);
                        }
                        while (!result.EndOfMessage);

                        ms.Seek(0, SeekOrigin.Begin);

                        if (result.MessageType == WebSocketMessageType.Text)
                        {
                            using (var reader = new StreamReader(ms, Encoding.UTF8))
                            {
                                // do stuff
                                var message = reader.ReadToEnd();
                                HandleMessage(message, client);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    //Console.WriteLine("Exception with message receive");
                }
            }
        }

        static void HandleMessage(string message, Client client)
        {
            var command = message.Split('#')[0];
            var args = message.Split('#')[1].Replace(" ", string.Empty).Split(',').ToDictionary(k => k.Split(':')[0], v => v.Split(':')[1]);

            try
            {
                switch (command)
                {
                    case "login":
                        if (args["password"] == Settings.Presets.ServerPassword)
                        {
                            client.Role = Client.ClientRole.Admin;
                            client.SendMessageAsync("Successfully logged");
                        }
                        else
                        {
                            client.SendMessageAsync("Wrong password!");
                        }
                        break;


                    default:

                        //Only for admins
                        if (client.Role == Client.ClientRole.Admin)
                        {
                            switch (command)
                            {
                                case "setCharactersMaxCount":
                                    Settings.Presets.CharactersMaxCount = int.Parse(args["count"]);
                                    break;

                                default:
                                    break;
                            }
                        }
                        else
                        {
                            client.SendMessageAsync("Command is unknown or requires Admin status.");
                        }

                        break;
                }
            }
            catch (Exception)
            {
                client.SendMessageAsync("Something went wrong. Check the correctness of the command and arguments. \n" +
                    "message format: \"command# arg1Name:arg1, arg2Name:arg2, ... arg[N]Name:arg[N]\"");
            }

        }

        private static void Update()
        {
            var coordinates = ModulesLibrary.DrawableData.ToList().Select(c => MathExtensions.UTM2Deg(c.ToVector2()))
                .Select(v => new[] { v.Y, v.X }).ToList();
            var jsonData = JsonConvert.SerializeObject(coordinates);

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

            Thread thread = new Thread(HandleClient);
            thread.Start(client);

            Console.WriteLine("Client connected");
        }
    }
}
