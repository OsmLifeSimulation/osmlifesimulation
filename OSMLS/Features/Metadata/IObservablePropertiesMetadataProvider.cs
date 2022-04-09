using System;
using System.Collections.Immutable;
using OSMLS.Features.Properties;

namespace OSMLS.Features.Metadata
{
	public interface IObservablePropertiesMetadataProvider
	{
		IImmutableDictionary<Type, IObservablePropertiesManager> TypesToObservablePropertiesManagers { get; }
	}
}