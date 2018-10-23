using System;
using System.Collections.Generic;
using System.Runtime.Loader;
using System.Text;
using System.Threading;

namespace OSM
{
    class Program
    {
        static OSM OSM;
        static Timer UpdateTimer;
        static void Main(string[] args)
        {
            //Events for systemd service
            AssemblyLoadContext.Default.Unloading += SigTermEventHandler; //register sigterm event handler. 
            Console.CancelKeyPress += CancelHandler; //register sigint event handler

            OSM = new OSM();

            var startTimeSpan = TimeSpan.Zero;
            var periodTimeSpan = TimeSpan.FromMilliseconds(50);
            UpdateTimer = new Timer((e) =>
            {
                OSM.Update();
            }, null, startTimeSpan, periodTimeSpan);

            Console.WriteLine("Server started");

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
