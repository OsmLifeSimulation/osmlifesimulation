using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Google.Protobuf;
using OSMLS.Map;
using OSMLS.Types.Model.Properties;
using OSMLSGlobalLibrary.Observable.Property;

namespace OSMLS.Features.Properties
{
	public class ObservablePropertiesManager : IObservablePropertiesManager
	{
		public ObservablePropertiesManager(IEnumerable<IInjectedProperty> properties)
		{
			_ObservablePropertiesByNames = properties
				.Where(property => property.IsObservable)
				.ToImmutableDictionary(
					property => property.Name,
					property => property
				);

			_ObservablePropertiesToAttributes = _ObservablePropertiesByNames.Values
				.ToImmutableDictionary(
					property => property,
					property => property.ObservablePropertyAttribute
				);

			_EditableObservablePropertiesByDescriptors = _ObservablePropertiesToAttributes
				.Where(propertyToObservablePropertyAttribute =>
					propertyToObservablePropertyAttribute.Value.Editable)
				.ToImmutableDictionary(
					propertyToObservablePropertyAttribute => propertyToObservablePropertyAttribute.Value.Title,
					propertyToObservablePropertyAttribute => propertyToObservablePropertyAttribute.Key
				);
		}

		private readonly ImmutableDictionary<string, IInjectedProperty> _ObservablePropertiesByNames;

		private readonly ImmutableDictionary<IInjectedProperty, ObservablePropertyAttribute>
			_ObservablePropertiesToAttributes;

		private readonly ImmutableDictionary<string, IInjectedProperty> _EditableObservablePropertiesByDescriptors;

		private static readonly
			IImmutableDictionary<Type, MapFeaturesMetadata.Types.ObservablePropertyMetadata.Types.ValueType>
			TypesToObservablePropertiesTypes =
				new Dictionary<Type, MapFeaturesMetadata.Types.ObservablePropertyMetadata.Types.ValueType>
				{
					{ typeof(double), MapFeaturesMetadata.Types.ObservablePropertyMetadata.Types.ValueType.Double },
					{ typeof(float), MapFeaturesMetadata.Types.ObservablePropertyMetadata.Types.ValueType.Float },
					{ typeof(int), MapFeaturesMetadata.Types.ObservablePropertyMetadata.Types.ValueType.Int32 },
					{ typeof(long), MapFeaturesMetadata.Types.ObservablePropertyMetadata.Types.ValueType.Int64 },
					{ typeof(uint), MapFeaturesMetadata.Types.ObservablePropertyMetadata.Types.ValueType.Uint32 },
					{ typeof(ulong), MapFeaturesMetadata.Types.ObservablePropertyMetadata.Types.ValueType.Uint64 },
					{ typeof(bool), MapFeaturesMetadata.Types.ObservablePropertyMetadata.Types.ValueType.Bool },
					{ typeof(string), MapFeaturesMetadata.Types.ObservablePropertyMetadata.Types.ValueType.String },
					{ typeof(ByteString), MapFeaturesMetadata.Types.ObservablePropertyMetadata.Types.ValueType.Bytes }
				}.ToImmutableDictionary();

		public IEnumerable<MapFeaturesMetadata.Types.ObservablePropertyMetadata> GetAllObservablePropertiesMetadata() =>
			_ObservablePropertiesToAttributes.Select(propertyToAttribute =>
				{
					var (property, attribute) = propertyToAttribute;

					return new MapFeaturesMetadata.Types.ObservablePropertyMetadata
					{
						Title = attribute.Title,
						Editable = attribute.Editable,
						ValueType = TypesToObservablePropertiesTypes[property.PropertyType]
					};
				}
			);

		public IEnumerable<ObservableProperty> GetAllObservableProperties(object obj) =>
			_ObservablePropertiesToAttributes.Select(
				propertyToAttribute =>
					GetObservableProperty(obj, propertyToAttribute.Key, propertyToAttribute.Value)
			);

		public bool IsPropertyObservable(string propertyName) =>
			_ObservablePropertiesByNames.ContainsKey(propertyName);

		public ObservableProperty GetObservableProperty(object obj, string propertyName)
		{
			var property = _ObservablePropertiesByNames[propertyName];

			return GetObservableProperty(
				obj,
				property,
				_ObservablePropertiesToAttributes[property]
			);
		}

		private static ObservableProperty GetObservableProperty(
			object obj,
			IInjectedProperty property,
			ObservablePropertyAttribute observablePropertyAttribute)
		{
			var observableProperty = new ObservableProperty
			{
				Title = observablePropertyAttribute.Title,
				Value = new ObservablePropertyValue()
			};

			switch (property.GetValue(obj))
			{
				case double doubleValue:
					observableProperty.Value.Double = doubleValue;
					break;
				case float floatValue:
					observableProperty.Value.Float = floatValue;
					break;
				case int intValue:
					observableProperty.Value.Int32 = intValue;
					break;
				case long longValue:
					observableProperty.Value.Int64 = longValue;
					break;
				case uint uintValue:
					observableProperty.Value.Uint32 = uintValue;
					break;
				case ulong ulongValue:
					observableProperty.Value.Uint64 = ulongValue;
					break;
				case bool boolValue:
					observableProperty.Value.Bool = boolValue;
					break;
				case string stringValue:
					observableProperty.Value.String = stringValue;
					break;
				case ByteString byteStringValue:
					observableProperty.Value.Bytes = byteStringValue;
					break;
				default:
					throw new ArgumentException(
						$"Property {property.Name} is of type {property.PropertyType}, which cannot be used for observable properties.",
						nameof(obj)
					);
			}

			return observableProperty;
		}

		public void SetObservableProperty(object obj, ObservableProperty observablePropertyUpdate) =>
			SetObservableProperty(
				obj,
				_EditableObservablePropertiesByDescriptors[observablePropertyUpdate.Title],
				observablePropertyUpdate.Value
			);

		private static void SetObservableProperty(
			object obj,
			IInjectedProperty property,
			ObservablePropertyValue observablePropertyValue)
		{
			// ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
			switch (observablePropertyValue.ValueCase)
			{
				case ObservablePropertyValue.ValueOneofCase.Double:
					property.SetValue(obj, observablePropertyValue.Double);
					break;
				case ObservablePropertyValue.ValueOneofCase.Float:
					property.SetValue(obj, observablePropertyValue.Float);
					break;
				case ObservablePropertyValue.ValueOneofCase.Int32:
					property.SetValue(obj, observablePropertyValue.Int32);
					break;
				case ObservablePropertyValue.ValueOneofCase.Int64:
					property.SetValue(obj, observablePropertyValue.Int64);
					break;
				case ObservablePropertyValue.ValueOneofCase.Uint32:
					property.SetValue(obj, observablePropertyValue.Uint32);
					break;
				case ObservablePropertyValue.ValueOneofCase.Uint64:
					property.SetValue(obj, observablePropertyValue.Uint64);
					break;
				case ObservablePropertyValue.ValueOneofCase.Bool:
					property.SetValue(obj, observablePropertyValue.Bool);
					break;
				case ObservablePropertyValue.ValueOneofCase.String:
					property.SetValue(obj, observablePropertyValue.String);
					break;
				case ObservablePropertyValue.ValueOneofCase.Bytes:
					property.SetValue(obj, observablePropertyValue.Bytes);
					break;
				case ObservablePropertyValue.ValueOneofCase.None:
					break;
			}
		}
	}
}