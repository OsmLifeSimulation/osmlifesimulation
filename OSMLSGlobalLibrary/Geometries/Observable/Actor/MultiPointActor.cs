using System;
using System.ComponentModel;

namespace OSMLSGlobalLibrary.Geometries.Observable.Actor
{
	public class MultiPointActor: ObservableMultiPoint, IActor
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public Guid Id { get; } = Guid.NewGuid();

		public MultiPointActor(ObservablePoint[] points) : base(points)
		{
		}
	}
}