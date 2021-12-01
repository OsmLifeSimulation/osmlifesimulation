using System;
using NetTopologySuite.Geometries;
using OSMLSGlobalLibrary.Observable.Geometries.Implementation;

namespace OSMLSGlobalLibrary.Observable.Geometries
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

		public ObservablePoint(double x, double y) : this(new Coordinate(x, y))
		{
		}
	}
}