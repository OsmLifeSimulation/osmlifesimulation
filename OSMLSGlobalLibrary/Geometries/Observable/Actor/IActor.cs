using System;
using System.ComponentModel;

namespace OSMLSGlobalLibrary.Geometries.Observable.Actor
{
	public interface IActor : INotifyCoordinatesChanged, INotifyPropertyChanged
	{
		Guid Id { get; }
	}
}