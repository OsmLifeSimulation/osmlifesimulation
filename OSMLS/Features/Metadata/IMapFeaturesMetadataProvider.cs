using System;
using System.Collections.Generic;
using OSMLS.Map;

namespace OSMLS.Features.Metadata
{
	public interface IMapFeaturesMetadataProvider
	{
		IEnumerable<MapFeaturesMetadata> GetMapFeaturesMetadata();

		IObservable<MapFeaturesMetadata> MapFeaturesMetadataObservable { get; }
	}
}