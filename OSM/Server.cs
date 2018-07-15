using Global.NetworkData;
using Microsoft.Xna.Framework;
using Global;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OSM.Simulated_Objects;

namespace OSM
{
    public static class Server
    {
        class Client
        {
            public Socket Socket { get; set; }
            public int ID { get; set; }
            public Client(Socket socket)
            {
                Socket = socket;
            }
        }
        static Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
        public static BinaryFormatter formatter = new BinaryFormatter();
        static List<Client> clients = new List<Client>();

        static Random random = new Random();

        public static List<LineData> LinesData = new List<LineData>();
        static Vector2 CameraOffset;

        public static void Init(Vector2 cameraOffset)
        {
            CameraOffset = cameraOffset;

            socket.Bind(new IPEndPoint(IPAddress.Any, 2048));
            socket.Listen(0);

            socket.BeginAccept(AcceptCallback, null);

            Console.ReadLine();
        }

        static void AcceptCallback(IAsyncResult ar)
        {
            Client client = new Client(socket.EndAccept(ar));
            Thread thread = new Thread(HandleClient);
            thread.Start(client);

            clients.Add(client);
            Console.WriteLine("Новое подключение");

            socket.BeginAccept(AcceptCallback, null);
        }

        static void HandleClient(object o)
        {
            Client client = (Client)o;
            MemoryStream ms = new MemoryStream(new byte[256], 0, 256, true, true);
            BinaryWriter writer = new BinaryWriter(ms);
            BinaryReader reader = new BinaryReader(ms);

            while (true)
            {
                ms.Position = 0;

                try
                {
                    client.Socket.Receive(ms.GetBuffer());
                }
                catch
                {
                    client.Socket.Shutdown(SocketShutdown.Both);
                    client.Socket.Disconnect(true);
                    clients.Remove(client);
                    Console.WriteLine($"Пользователь с id {client.ID} отключился");
                    return;
                }


                int code = reader.ReadInt32();

                switch ((PacketInfo)code)
                {
                    case PacketInfo.ID:
                        while (true)
                        {
                            int id = random.Next(0, 1001);
                            if (clients.Find(c => c.ID == id) == null)
                            {
                                writer.Write(id);
                                client.Socket.Send(ms.GetBuffer());
                                client.ID = id;
                                break;
                            }
                        }
                        break;
                    case PacketInfo.CameraOffset:
                        ms.Clear();
                        writer.Write((int)PacketInfo.CameraOffset);
                        formatter.Serialize(ms, new PointData(CameraOffset));
                        client.Socket.Send(ms.GetBuffer());

                        break;
                    case PacketInfo.Map:
                        foreach (var lineData in LinesData)
                        {
                            ms.Clear();
                            writer.Write((int)PacketInfo.Map);
                            formatter.Serialize(ms, lineData);
                            client.Socket.Send(ms.GetBuffer());
                        }

                        break;
                }
            }
        }
        public static void UpdateClientsWithCharacter(Character character)
        {
            //new Thread(() =>
            //{
                MemoryStream ms = new MemoryStream(new byte[256], 0, 256, true, true);
                BinaryWriter writer = new BinaryWriter(ms);
                BinaryReader reader = new BinaryReader(ms);

                foreach (var client in clients)
                {
                    ms.Clear();
                    writer.Write((int)PacketInfo.Character);
                    formatter.Serialize(ms, character.PointData);
                    client.Socket.Send(ms.GetBuffer());
                }
            //});
        }

        static void Clear(this MemoryStream source)
        {
            byte[] buffer = source.GetBuffer();
            Array.Clear(buffer, 0, buffer.Length);
            source.Position = 0;
            source.SetLength(0);
        }
    }
}
