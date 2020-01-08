using GeoJSON.Net.Geometry;
using System.Collections.Generic;
using System.Linq;

namespace OSMLSGlobalLibrary.Map.MapObjects
{
    [MapObject(
        @"stroke: new ol.style.Stroke({
			color: 'rgba(0, 0, 0, 0.4)',
			width: 1
		})")]
    public class MapLineString<PointType> : MapObject where PointType : IPoint
    {
        public List<PointType> Points { get; }
        public MapLineString(List<PointType> points)
        {
            Points = points;
        }
        public override IGeometryObject Geometry
        {
            get { return new LineString(Points.Select(point => new Position(point.Y, point.X))); }
        }
    }
}
