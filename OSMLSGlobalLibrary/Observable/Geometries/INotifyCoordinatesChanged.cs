using System;

namespace OSMLSGlobalLibrary.Observable.Geometries
{
	public interface INotifyCoordinatesChanged
	{
		event EventHandler CoordinatesChanged;
	}
}