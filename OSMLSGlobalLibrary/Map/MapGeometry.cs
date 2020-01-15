using NetTopologySuite.Features;
using NetTopologySuite.Geometries;

namespace OSMLSGlobalLibrary.Map
{
    public class MapGeometry<GeometryType> : MapObject where GeometryType : Geometry
    {
        public GeometryType Geometry { get; }

        public AttributesTable Attributes { get; } = new AttributesTable();

        public MapGeometry(GeometryType geometry)
        {
            Geometry = geometry;
        }

        public sealed override Feature Feature { get { return new Feature(Geometry, Attributes); } }
    }
}
