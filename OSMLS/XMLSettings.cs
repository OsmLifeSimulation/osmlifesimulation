using System.Xml.Serialization;

namespace OSMLS
{
    [XmlRoot(ElementName = "Presets")]
    public class PresetsXml
    {
        public string OsmFilePath { get; set; } = "maps/map.osm";
        public bool RunWebSocketServer { get; set; } = true;
        public string WebSocketServerUri { get; set; } = "http://localhost:61104/";
    }
}
