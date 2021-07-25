using System;
using NetTopologySuite.Geometries;
using OSMLSGlobalLibrary.Geometries.Observable.Implementation;

namespace OSMLSGlobalLibrary.Geometries.Observable
{
	public class ObservableLinearRing : LinearRing, INotifyCoordinatesChanged
	{
		public event EventHandler CoordinatesChanged = delegate { };

		public ObservableLinearRing(Coordinate[] coordinates) : base(
			ObservableCoordinateGeometryFactoryProvider.Instance.CoordinateSequenceFactory.Create(coordinates),
			ObservableCoordinateGeometryFactoryProvider.Instance)
		{
			((ObservableCoordinateArraySequence) CoordinateSequence).OrdinateChanged +=
				(sender, args) => CoordinatesChanged.Invoke(this, EventArgs.Empty);
		}
	}
}