using System;
using System.Collections.Immutable;
using OSMLS.Map.Properties;

namespace OSMLS.Map.Metadata
{
	public interface IObservablePropertiesMetadataProvider
	{
		IImmutableDictionary<Type, ObservablePropertiesManager> TypesToObservablePropertiesManagers { get; }
	}
}