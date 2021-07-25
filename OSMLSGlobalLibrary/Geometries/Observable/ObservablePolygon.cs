using System;
using System.Linq;
using NetTopologySuite.Geometries;

namespace OSMLSGlobalLibrary.Geometries.Observable
{
	public class ObservablePolygon : Polygon, INotifyCoordinatesChanged
	{
		public event EventHandler CoordinatesChanged = delegate { };

		public ObservablePolygon(ObservableLinearRing shell, ObservableLinearRing[] holes = null)
			: base(shell, holes?.Cast<LinearRing>().ToArray(), ObservableCoordinateGeometryFactoryProvider.Instance)
		{
			shell.CoordinatesChanged += (sender, args) => CoordinatesChanged.Invoke(this, EventArgs.Empty);

			if (holes == null) return;

			foreach (var hole in holes)
				hole.CoordinatesChanged += (sender, args) => CoordinatesChanged.Invoke(this, EventArgs.Empty);
		}
	}
}