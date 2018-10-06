using System;
using System.Collections.Generic;
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
            OSM = new OSM();

            var startTimeSpan = TimeSpan.Zero;
            var periodTimeSpan = TimeSpan.FromMilliseconds(50);
            UpdateTimer = new Timer((e) =>
            {
                OSM.Update();
            }, null, startTimeSpan, periodTimeSpan);

            Console.WriteLine("Server started");
            Console.ReadKey();
        }
    }
}
