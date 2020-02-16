using System.Xml.Serialization;

namespace OSMLS
{
    [XmlRoot(ElementName = "Presets")]
    public class PresetsXml
    {
        public string OsmFilePath { get; set; } = "maps/Map.osm";
        public bool RunWebSocketServer { get; set; } = true;
        public int WebSocketServerPort { get; set; } = 8080;
    }
}
