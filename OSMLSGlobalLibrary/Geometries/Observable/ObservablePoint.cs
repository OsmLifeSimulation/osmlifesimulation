using System;
using NetTopologySuite.Geometries;
using OSMLSGlobalLibrary.Geometries.Observable.Implementation;

namespace OSMLSGlobalLibrary.Geometries.Observable
{
	public class ObservablePoint : Point, INotifyCoordinatesChanged
	{
		public event EventHandler CoordinatesChanged = delegate { };

		public ObservablePoint(Coordinate coordinate) : base(
			ObservableCoordinateGeometryFactoryProvider.Instance.CoordinateSequenceFactory.Create(new[]
				{coordinate}),
			ObservableCoordinateGeometryFactoryProvider.Instance)
		{
			((ObservableCoordinateArraySequence) CoordinateSequence).OrdinateChanged +=
				(sender, args) => CoordinatesChanged.Invoke(this, EventArgs.Empty);
		}

		// public PointActor(double x, double y, double z) : this(new CoordinateZ(x, y, z))
		// {
		// }

		public ObservablePoint(double x, double y) : this(new Coordinate(x, y))
		{
		}
	}
}