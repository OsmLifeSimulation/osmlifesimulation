using OSMLSGlobalLibrary;
using System.Diagnostics;

namespace OSMLS
{
    class OSM
    {
        private Stopwatch TimeNow { get; } = new Stopwatch();

        private ModulesLibrary ModulesLibrary { get; }

        public MapObjectsCollection MapObjects { get; } = new MapObjectsCollection();

        public OSM(string osmFilePath)
        {
            TimeNow.Start();

            var rawData = Constants.DeserializeXml<OsmXml>(osmFilePath);
            ModulesLibrary = new ModulesLibrary(rawData, MapObjects);
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        public void Update()
        {
            foreach (var module in ModulesLibrary.Modules)
            {
                module.Value.Update(TimeNow.ElapsedMilliseconds);
            }
        }

    }

}
