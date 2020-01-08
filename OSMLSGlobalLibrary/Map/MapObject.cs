using GeoJSON.Net;
using GeoJSON.Net.Geometry;

namespace OSMLSGlobalLibrary.Map
{
    [MapObject(null)]
    public abstract class MapObject
    {
        public abstract IGeometryObject Geometry { get; }
    }
}