using System;
using System.Collections.Immutable;
using OSMLS.Map.Features.Properties;

namespace OSMLS.Map.Features.Metadata
{
	public interface IObservablePropertiesMetadataProvider
	{
		IImmutableDictionary<Type, ObservablePropertiesManager> TypesToObservablePropertiesManagers { get; }
	}
}