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

        //Set less than 0 to make it infinite
        public int CharactersMaxCount { get; set; } = 50;

        public bool RunWebSocketServer { get; set; } = true;
        public int WebSocketServerPort { get; set; } = 8080;
        public string ServerPassword { get; set; } = "osm";
    }
}
