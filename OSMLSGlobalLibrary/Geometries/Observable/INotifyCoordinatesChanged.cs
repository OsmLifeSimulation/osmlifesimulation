using System;

namespace OSMLSGlobalLibrary.Geometries.Observable
{
	public interface INotifyCoordinatesChanged
	{
		event EventHandler CoordinatesChanged;
	}
}