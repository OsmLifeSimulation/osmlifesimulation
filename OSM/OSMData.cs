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
        private OsmXml rawData { get; set; }

        public Rectangle area;

        public List<Line> BuildingLines { get; private set; } = new List<Line>();
        public List<Line> RoadLines { get; private set; } = new List<Line>();

        List<List<Vector2>> buildingPoints { get; set; }
        List<List<Vector2>> roadPoints { get; set; }
        List<Vector2> nodes { get; set; }

        public List<Point> Entrances = new List<Point>();

        public Graph MovementsGraph { get; private set; }

        public OSMData()
        {
            deserializeXml();

            buildingPoints = rawData.Way.Where(w => w.Tag.Exists(t => t.K == "building" && t.V == "yes"))
                .Select(w => w.Nd.Select(n => MathExtensions.Deg2UTM(rawData.Node.First(node => node.Id == n.Ref))).ToList()).ToList();

            roadPoints = rawData.Way.Where(w => w.Tag.Exists(t => t.K == "highway"))
                .Select(w => w.Nd.Select(n => MathExtensions.Deg2UTM(rawData.Node.First(node => node.Id == n.Ref))).ToList()).ToList();

            nodes = rawData.Node.Where(n => n.Tag.Any()).Select(n => MathExtensions.Deg2UTM(n)).ToList();

            foreach (var build in buildingPoints)
            {
                for (int i = 0; i < build.Count; i++)
                {
                    int next = i == build.Count - 1 ? 0 : i + 1;
                    BuildingLines.Add(new Line(build[i], build[next]));
                }
            }
            foreach (var road in roadPoints)
            {
                for (int i = 0; i < road.Count; i++)
                {
                    if (i != road.Count - 1)
                    {
                        RoadLines.Add(new Line(road[i], road[i + 1]));
                    }
                }
            }

            //create Entrances
            foreach (var building in buildingPoints)
            {
                var index = Constants.rnd.Next(building.Count - 2);
                Entrances.Add(MathExtensions.LineCenter(new Line(building[index], building[index + 1])).ToPoint());
            }

            var minLatLon = MathExtensions.Deg2UTM(double.Parse(rawData.Bounds.Maxlat), double.Parse(rawData.Bounds.Minlon)).ToPoint();
            var maxLatLon = MathExtensions.Deg2UTM(double.Parse(rawData.Bounds.Minlat), double.Parse(rawData.Bounds.Maxlon)).ToPoint();

            area = new Rectangle(minLatLon - Constants.AreaExtension,
                maxLatLon - minLatLon + (Constants.AreaExtension + Constants.AreaExtension));


            MovementsGraph = new Graph(area, BuildingLines, RoadLines);

        }

        private void deserializeXml()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(OsmXml));
            StreamReader reader = new StreamReader(Constants.OsmFilePath);
            rawData = (OsmXml)serializer.Deserialize(reader);
            reader.Close();
        }
    }
}
