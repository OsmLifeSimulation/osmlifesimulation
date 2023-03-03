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

		protected bool Equals(PolygonActor other)
		{
			return base.Equals(other) && Id.Equals(other.Id);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;
			if (ReferenceEquals(this, obj))
				return true;
			// ReSharper disable once ConvertIfStatementToReturnStatement
			if (obj.GetType() != GetType())
				return false;

			return Equals((PolygonActor)obj);
		}

		public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), Id);
	}
}