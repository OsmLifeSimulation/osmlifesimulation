using System;
using NetTopologySuite.Geometries;

namespace OSMLSGlobalLibrary.Geometries.Observable
{
	public class ObservableGeometryCollection : GeometryCollection, INotifyCoordinatesChanged
	{
		public event EventHandler CoordinatesChanged = delegate { };

		public ObservableGeometryCollection(Geometry[] geometries)
			: base(geometries, ObservableCoordinateGeometryFactoryProvider.Instance)
		{
			foreach (var geometry in geometries)
				if (geometry is INotifyCoordinatesChanged notifyGeometry)
					notifyGeometry.CoordinatesChanged +=
						(sender, args) => CoordinatesChanged.Invoke(this, EventArgs.Empty);
				else
					throw new ArgumentException("Provided geometry is not observable.");
		}
	}
}