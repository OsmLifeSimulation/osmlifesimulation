using Microsoft.Xna.Framework;
using OSM.Structures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OSM
{
    class OSMData
    {
        private Osm rawData { get; set; }

        public List<List<Vector2>> buildings { get; private set; } = new List<List<Vector2>>();
        public List<List<Vector2>> roads { get; private set; } = new List<List<Vector2>>();
        public List<Vector2> nodes = new List<Vector2>();
        public OSMData()
        {
            serializeXml();

            buildings = rawData.Way.Where(w => w.Tag.Exists(t => t.K == "building" && t.V == "yes"))
                .Select(w => w.Nd.Select(n => MathExtensions.Deg2UTM(rawData.Node.First(node => node.Id == n.Ref))).ToList()).ToList();

            roads = rawData.Way.Where(w => w.Tag.Exists(t => t.K == "highway"))
                .Select(w => w.Nd.Select(n => MathExtensions.Deg2UTM(rawData.Node.First(node => node.Id == n.Ref))).ToList()).ToList();

            nodes = rawData.Node.Where(n => n.Tag.Any()).Select(n => MathExtensions.Deg2UTM(n)).ToList();
        }

        private void serializeXml()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Osm));
            StreamReader reader = new StreamReader(Constants.OsmFilePath);
            rawData = (Osm)serializer.Deserialize(reader);
            reader.Close();
        }
    }
}
