using System;
using System.ComponentModel;

namespace OSMLSGlobalLibrary.Geometries.Observable.Actor
{
	public class MultiPolygonActor : ObservableMultiPolygon, IActor
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public Guid Id { get; } = Guid.NewGuid();

		public MultiPolygonActor(ObservablePolygon[] polygons) : base(polygons)
		{
		}
	}
}