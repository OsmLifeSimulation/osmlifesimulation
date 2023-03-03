﻿using System;
using NetTopologySuite.Geometries;
using OSMLSGlobalLibrary.Observable.Geometries.Implementation;

namespace OSMLSGlobalLibrary.Observable.Geometries
{
	public class ObservableLineString : LineString, INotifyCoordinatesChanged
	{
		public event EventHandler CoordinatesChanged = delegate { };

		public ObservableLineString(Coordinate[] coordinates) : base(
			ObservableCoordinateGeometryFactoryProvider.Instance.CoordinateSequenceFactory.Create(coordinates),
			ObservableCoordinateGeometryFactoryProvider.Instance)
		{
			((ObservableCoordinateArraySequence) CoordinateSequence).OrdinateChanged +=
				(sender, args) => CoordinatesChanged.Invoke(this, EventArgs.Empty);
		}
	}
}