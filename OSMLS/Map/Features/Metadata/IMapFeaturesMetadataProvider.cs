using System;
using System.Collections.Generic;

namespace OSMLS.Map.Features.Metadata
{
	public interface IMapFeaturesMetadataProvider
	{
		IEnumerable<MapFeaturesMetadata> GetMapFeaturesMetadata();

		IObservable<MapFeaturesMetadata> MapFeaturesMetadataObservable { get; }
	}
}