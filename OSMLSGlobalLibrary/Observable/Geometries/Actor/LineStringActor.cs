using System;
using System.ComponentModel;
using NetTopologySuite.Geometries;

namespace OSMLSGlobalLibrary.Observable.Geometries.Actor
{
	public class LineStringActor: ObservableLineString, IActor
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public Guid Id { get; } = Guid.NewGuid();

		public LineStringActor(Coordinate[] coordinates) : base(coordinates)
		{
		}
	}
}