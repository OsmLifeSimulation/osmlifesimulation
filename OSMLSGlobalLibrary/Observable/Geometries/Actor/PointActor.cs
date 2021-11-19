using System;
using System.ComponentModel;
using NetTopologySuite.Geometries;

namespace OSMLSGlobalLibrary.Observable.Geometries.Actor
{
	public class PointActor : ObservablePoint, IActor
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public Guid Id { get; } = Guid.NewGuid();

		public PointActor(Coordinate coordinate) : base(coordinate)
		{
		}

		public PointActor(double x, double y) : base(x, y)
		{
		}
	}
}