using OSMLSGlobalLibrary;
using System.Diagnostics;

namespace OSM
{
    class OSM
    {
        Stopwatch TimeNow = new Stopwatch();

        OsmXml RawData;

        ModulesLibrary ModulesLibrary { get; set; }

        MapObjectsCollection MapObjects { get; } = new MapObjectsCollection();

        public OSM()
        {
            TimeNow.Start();
            Settings.Init();

            RawData = Constants.DeserializeXml<OsmXml>(Constants.OsmFolderPath + Settings.Presets.OsmFileName);
            ModulesLibrary = new ModulesLibrary(RawData, MapObjects);

            if (Settings.Presets.RunWebSocketServer)
                ServerWebSocket.Init(ModulesLibrary, MapObjects);
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
