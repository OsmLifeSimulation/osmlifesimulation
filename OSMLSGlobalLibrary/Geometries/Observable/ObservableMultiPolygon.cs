using System;
using System.Linq;
using NetTopologySuite.Geometries;

namespace OSMLSGlobalLibrary.Geometries.Observable
{
	public class ObservableMultiPolygon : MultiPolygon, INotifyCoordinatesChanged
	{
		public event EventHandler CoordinatesChanged = delegate { };

		public ObservableMultiPolygon(ObservablePolygon[] polygons)
			: base(polygons.Cast<Polygon>().ToArray(), ObservableCoordinateGeometryFactoryProvider.Instance)
		{
			foreach (var polygon in polygons)
				polygon.CoordinatesChanged += (sender, args) => CoordinatesChanged.Invoke(this, EventArgs.Empty);
		}
	}
}