using NetTopologySuite.Geometries;
using OSMLSGlobalLibrary.Geometries.Observable.Implementation;

namespace OSMLSGlobalLibrary.Geometries.Observable
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