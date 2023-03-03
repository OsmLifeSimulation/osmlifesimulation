using System.Collections.Generic;
using OSMLS.Map;

namespace OSMLS.Features.Properties
{
	public interface IObservablePropertiesManager
	{
		public IEnumerable<MapFeaturesMetadata.Types.ObservablePropertyMetadata> GetAllObservablePropertiesMetadata();

		public IEnumerable<ObservableProperty> GetAllObservableProperties(object obj);

		public bool IsPropertyObservable(string propertyName);

		public ObservableProperty GetObservableProperty(object obj, string propertyName);

		public void SetObservableProperty(object obj, ObservableProperty observablePropertyUpdate);
	}
}