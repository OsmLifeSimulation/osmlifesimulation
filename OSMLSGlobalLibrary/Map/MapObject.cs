using NetTopologySuite.Features;

namespace OSMLSGlobalLibrary.Map
{
    public abstract class MapObject
    {
        public abstract Feature Feature { get; }
    }
}
