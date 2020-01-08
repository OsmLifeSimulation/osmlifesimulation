using GeoJSON.Net.Geometry;
using System.Collections.Generic;
using System.Linq;

namespace OSMLSGlobalLibrary.Map.MapObjects
{
    [MapObject(
        @"fill: new ol.style.Fill({
			color: 'rgba(255, 255, 255, 0.4)'
		}),
		stroke: new ol.style.Stroke({
			color: 'rgba(0, 0, 0, 0.4)',
			width: 1
		})")]
    public class MapPolygon<PointType> : MapObject where PointType : IPoint
    {
        public List<PointType> Points { get; }

        public MapPolygon(List<PointType> points)
        {
            Points = points;
        }

        public override IGeometryObject Geometry
        {
            get { return new Polygon(new List<LineString>() {
                new LineString(Points.Append(Points.First()).Select(point => new Position(point.Y, point.X))) });
            }
        }
    }
}
