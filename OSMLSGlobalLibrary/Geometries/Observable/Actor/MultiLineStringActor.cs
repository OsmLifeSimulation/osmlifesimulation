using System;
using System.ComponentModel;
using OSMLSGlobalLibrary.Geometries.Observable;

namespace OSMLSGlobalLibrary.Geometries.Observable.Actor
{
	public class MultiLineStringActor: ObservableMultiLineString, IActor
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public Guid Id { get; } = Guid.NewGuid();

		public MultiLineStringActor(ObservableLineString[] lineStrings) : base(lineStrings)
		{
		}
	}
}