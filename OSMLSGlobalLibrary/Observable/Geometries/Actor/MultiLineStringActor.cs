using System;
using System.ComponentModel;
using OSMLSGlobalLibrary.Observable.Geometries;

namespace OSMLSGlobalLibrary.Observable.Geometries.Actor
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