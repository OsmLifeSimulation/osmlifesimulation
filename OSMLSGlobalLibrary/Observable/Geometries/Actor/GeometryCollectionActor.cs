using System;
using System.ComponentModel;
using NetTopologySuite.Geometries;

namespace OSMLSGlobalLibrary.Observable.Geometries.Actor
{
	public class GeometryCollectionActor : ObservableGeometryCollection, IActor
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public Guid Id { get; } = Guid.NewGuid();

		public GeometryCollectionActor(Geometry[] geometries) : base(geometries)
		{
		}
	}
}