using System;
using System.Collections.Generic;

namespace OSMLS.Map.Metadata
{
	public interface IMapFeaturesMetadataProvider
	{
		IEnumerable<MapFeaturesMetadata> GetMapFeaturesMetadata();

		IObservable<MapFeaturesMetadata> MapFeaturesMetadataObservable { get; }
	}
}