using NetTopologySuite.Features;
using NetTopologySuite.Geometries;

namespace OSMLSGlobalLibrary.Map
{
    public abstract class MapObject
    {
        public abstract Geometry BaseGeometry { get; }

        public AttributesTable Attributes { get; } = new AttributesTable();

        public Feature Feature => new Feature(BaseGeometry, Attributes);
    }
}
