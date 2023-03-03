using NetTopologySuite.Geometries;
using OSMLSGlobalLibrary.Observable.Geometries.Implementation;

namespace OSMLSGlobalLibrary.Observable.Geometries
{
	internal static class ObservableCoordinateGeometryFactoryProvider
	{
		public static readonly GeometryFactory Instance = new GeometryFactory(
			new PrecisionModel(),
			0,
			ObservableCoordinateSequenceFactory.Instance
		);
	}
}