using System;
using System.Collections.Generic;

namespace OSMLS.Map.Features
{
	public interface IMapFeaturesProvider
	{
		IEnumerable<MapFeature> GetMapFeatures();
		IObservable<MapFeature> MapFeaturesObservable { get; }
		IObservable<RemoveMapFeatureEvent> RemoveMapFeatureEventsObservable { get; }
	}
}