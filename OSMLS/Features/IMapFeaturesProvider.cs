using System;
using System.Collections.Generic;
using OSMLS.Map;

namespace OSMLS.Features
{
	public interface IMapFeaturesProvider
	{
		IEnumerable<MapFeature> GetMapFeatures();
		IObservable<MapFeature> MapFeaturesObservable { get; }
		IObservable<RemoveMapFeatureEvent> RemoveMapFeatureEventsObservable { get; }
	}
}