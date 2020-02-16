using System.Diagnostics;

namespace OSMLS
{
    internal class OsmLifeSimulator
    {
        private Stopwatch TimeNow { get; } = new Stopwatch();

        private ModulesLibrary ModulesLibrary { get; }

        public MapObjectsCollection MapObjects { get; } = new MapObjectsCollection();

        public OsmLifeSimulator(string osmFilePath)
        {
            TimeNow.Start();

            ModulesLibrary = new ModulesLibrary(osmFilePath, MapObjects);
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
