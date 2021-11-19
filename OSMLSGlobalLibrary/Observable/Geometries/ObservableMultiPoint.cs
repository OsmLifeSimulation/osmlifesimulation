using System;
using System.Linq;
using NetTopologySuite.Geometries;

namespace OSMLSGlobalLibrary.Observable.Geometries
{
	public class ObservableMultiPoint : MultiPoint, INotifyCoordinatesChanged
	{
		public event EventHandler CoordinatesChanged = delegate { };

		public ObservableMultiPoint(ObservablePoint[] points)
			: base(points.Cast<Point>().ToArray(), ObservableCoordinateGeometryFactoryProvider.Instance)
		{
			foreach (var point in points)
				point.CoordinatesChanged += (sender, args) => CoordinatesChanged.Invoke(this, EventArgs.Empty);
		}
	}
}