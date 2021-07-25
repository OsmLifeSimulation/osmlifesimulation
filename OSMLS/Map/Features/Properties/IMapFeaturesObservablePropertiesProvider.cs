using System;
using System.Collections.Generic;

namespace OSMLS.Map.Features.Properties
{
	public interface IMapFeaturesObservablePropertiesProvider
	{
		IEnumerable<MapFeatureObservableProperty> GetMapFeaturesObservableProperties();

		IObservable<MapFeatureObservableProperty> MapFeaturesObservablePropertiesObservable { get; }

		void SetMapFeatureObservableProperty(MapFeatureObservableProperty mapFeatureObservableProperty);
	}
}