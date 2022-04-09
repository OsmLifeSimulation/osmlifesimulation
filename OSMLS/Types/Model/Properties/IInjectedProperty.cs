using System;
using OSMLSGlobalLibrary.Observable.Property;

namespace OSMLS.Types.Model.Properties
{
	public interface IInjectedProperty
	{
		public string Name { get; }

		public bool IsObservable { get; }

		public ObservablePropertyAttribute ObservablePropertyAttribute { get; }

		public Type PropertyType { get; }

		public object GetValue(object obj);

		public void SetValue(object obj, object value);
	}
}