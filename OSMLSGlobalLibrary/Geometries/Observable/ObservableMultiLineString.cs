using System;
using System.Linq;
using NetTopologySuite.Geometries;

namespace OSMLSGlobalLibrary.Geometries.Observable
{
	public class ObservableMultiLineString : MultiLineString, INotifyCoordinatesChanged
	{
		public event EventHandler CoordinatesChanged = delegate { };

		public ObservableMultiLineString(ObservableLineString[] lineStrings)
			: base(lineStrings.Cast<LineString>().ToArray(), ObservableCoordinateGeometryFactoryProvider.Instance)
		{
			foreach (var lineString in lineStrings)
				lineString.CoordinatesChanged += (sender, args) => CoordinatesChanged.Invoke(this, EventArgs.Empty);
		}
	}
}