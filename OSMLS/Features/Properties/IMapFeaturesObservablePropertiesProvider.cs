using System;
using System.Collections.Generic;
using OSMLS.Map;

namespace OSMLS.Features.Properties
{
	public interface IMapFeaturesObservablePropertiesProvider
	{
		IEnumerable<MapFeatureObservableProperty> GetMapFeaturesObservableProperties();

		IObservable<MapFeatureObservableProperty> MapFeaturesObservablePropertiesObservable { get; }

		void SetMapFeatureObservableProperty(MapFeatureObservableProperty mapFeatureObservableProperty);
	}
}