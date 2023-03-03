using System;
using System.ComponentModel;

namespace OSMLSGlobalLibrary.Observable.Geometries.Actor
{
	public interface IActor : INotifyCoordinatesChanged, INotifyPropertyChanged
	{
		Guid Id { get; }
	}
}