using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace OSM
{
    class OSM
    {
        Stopwatch TimeNow = new Stopwatch();

        OSMData data;

        ModulesLibrary ModulesLibrary { get; set; }

        public OSM()
        {
            TimeNow.Start();
            Settings.Init();

            data = new OSMData();
            ModulesLibrary = new ModulesLibrary(data);

            if (Settings.Presets.RunWebSocketServer)
                ServerWebSocket.Init(ModulesLibrary);

            PathFinding.Init(data);
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        public void Update()
        {
            foreach (var module in ModulesLibrary.modules)
            {
                module.Value.Update(TimeNow.ElapsedMilliseconds);
            }
        }

    }

}
