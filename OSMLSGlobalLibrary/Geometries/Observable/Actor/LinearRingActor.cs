using System;
using System.ComponentModel;
using NetTopologySuite.Geometries;

namespace OSMLSGlobalLibrary.Geometries.Observable.Actor
{
	public class LinearRingActor: ObservableLinearRing, IActor
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public Guid Id { get; } = Guid.NewGuid();

		public LinearRingActor(Coordinate[] coordinates) : base(coordinates)
		{
		}
	}
}