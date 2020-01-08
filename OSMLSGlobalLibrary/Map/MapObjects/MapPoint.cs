using GeoJSON.Net.Geometry;

namespace OSMLSGlobalLibrary.Map.MapObjects
{
    [MapObject(
        @"image: new ol.style.Circle({
		    opacity: 1.0,
		    scale: 1.0,
		    radius: 3,
		    fill: new ol.style.Fill({
		      color: 'rgba(255, 255, 255, 0.4)'
		    }),
		    stroke: new ol.style.Stroke({
		      color: 'rgba(0, 0, 0, 0.4)',
		      width: 1
		    }),
	      })
        ")]
    public class MapPoint : MapObject, IPoint
    {
        public double X { get; set; }
        public double Y { get; set; }

        public MapPoint(double x, double y)
        {
            X = x;
            Y = y;
        }

        public override IGeometryObject Geometry
        {
            get { return new Point(new Position(Y, X)); }
        }
    }
}
