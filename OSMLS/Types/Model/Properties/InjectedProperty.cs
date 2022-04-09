using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OSMLSGlobalLibrary.Observable.Property;

namespace OSMLS.Types.Model.Properties
{
	public class InjectedProperty : IInjectedProperty
	{
		public InjectedProperty(PropertyInfo propertyInfo)
		{
			_PropertyInfo = propertyInfo;
		}

		public static IEnumerable<InjectedProperty> FromType(IInjectedType type) =>
			type.GetProperties().Select(property => new InjectedProperty(property));

		private readonly PropertyInfo _PropertyInfo;

		public string Name => _PropertyInfo.Name;

		public bool IsObservable => _PropertyInfo.IsDefined(typeof(ObservablePropertyAttribute), false);

		public ObservablePropertyAttribute ObservablePropertyAttribute =>
			(ObservablePropertyAttribute)_PropertyInfo.GetCustomAttribute(typeof(ObservablePropertyAttribute));

		public Type PropertyType => _PropertyInfo.PropertyType;

		public object GetValue(object obj) => _PropertyInfo.GetValue(obj);

		public void SetValue(object obj, object value) => _PropertyInfo.SetValue(obj, value);
	}
}