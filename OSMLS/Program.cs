using System;
using System.Runtime.Loader;
using System.Threading;

namespace OSMLS
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //Events for systemd service
            AssemblyLoadContext.Default.Unloading += SigTermEventHandler; //register sigterm event handler. 
            Console.CancelKeyPress += CancelHandler; //register sigint event handler

            var presets = Constants.DeserializeXmlOrCreateNew<PresetsXml>(Constants.PresetsPath);

            var osm = new OSM(presets.OsmFilePath);
            new Timer(e =>
            {
                osm.Update();
            }, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(50));

            if (presets.RunWebSocketServer)
            {
                var webSocketServer = new WebSocketServer(presets.WebSocketServerPort, osm.MapObjects);

                var startTimeSpan = TimeSpan.Zero;
                var periodTimeSpan = TimeSpan.FromSeconds(1);
                new Timer(callback =>
                {
                    webSocketServer.Update();
                }, null, startTimeSpan, periodTimeSpan);
            }

            Console.WriteLine("Application successfully started.");

            while (true)
            {
                Thread.Sleep(10000);
            }
        }

        private static void SigTermEventHandler(AssemblyLoadContext obj)
        {
            Console.WriteLine("Unloading...");
        }

        private static void CancelHandler(object sender, ConsoleCancelEventArgs e)
        {
            Console.WriteLine("Exiting...");
        }
    }
}
