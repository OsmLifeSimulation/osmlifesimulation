using NetTopologySuite.Geometries;

namespace OSMLSGlobalLibrary.Map
{
    public class MapGeometry<GeometryType> : MapObject where GeometryType : Geometry
    {
        public GeometryType Geometry { get; }

        public MapGeometry(GeometryType geometry)
        {
            Geometry = geometry;
        }

        public override Geometry BaseGeometry { get { return Geometry; } }
    }
}
