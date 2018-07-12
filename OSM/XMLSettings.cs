using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OSM
{
    [XmlRoot(ElementName = "Presets")]
    public class PresetsXml
    {
        //Threads that add new objects
        public int AdditionalThreadsCount { get; set; } = 1;

        public int GridFrequency { get; set; } = 20;

        public string OsmFileName { get; set; } = "Map.osm";
    }

    [XmlRoot(ElementName = "Controls")]
    public class ControlsXml
    {
        public Keys SwitchGreed = Keys.G;

        public List<Keys> Up { get; set; } = new List<Keys>() { Keys.Up, Keys.W };
        public List<Keys> Down { get; set; } = new List<Keys>() { Keys.Down, Keys.S };
        public List<Keys> Right { get; set; } = new List<Keys>() { Keys.Right, Keys.D };
        public List<Keys> Left { get; set; } = new List<Keys>() { Keys.Left, Keys.A };
    }
}
