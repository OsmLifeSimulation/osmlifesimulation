﻿using System;
using System.ComponentModel;

namespace OSMLSGlobalLibrary.Observable.Geometries.Actor
{
	public class MultiPolygonActor : ObservableMultiPolygon, IActor
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public Guid Id { get; } = Guid.NewGuid();

		public MultiPolygonActor(ObservablePolygon[] polygons) : base(polygons)
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