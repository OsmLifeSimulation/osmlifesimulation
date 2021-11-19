using System;
using System.ComponentModel;

namespace OSMLSGlobalLibrary.Observable.Geometries.Actor
{
	public class PolygonActor : ObservablePolygon, IActor
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public Guid Id { get; } = Guid.NewGuid();

		public PolygonActor(ObservableLinearRing shell, ObservableLinearRing[] holes = null) : base(shell, holes)
		{
		}
	}
}