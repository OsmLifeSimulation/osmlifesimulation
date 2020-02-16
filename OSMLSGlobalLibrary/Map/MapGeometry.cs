using NetTopologySuite.Geometries;

namespace OSMLSGlobalLibrary.Map
{
    [CustomStyle]
    public class MapGeometry<TGeometry> : MapObject where TGeometry : Geometry
    {
        public TGeometry Geometry { get; }

        public MapGeometry(TGeometry geometry)
        {
            Geometry = geometry;
        }

        public override Geometry BaseGeometry => Geometry;
    }
}
