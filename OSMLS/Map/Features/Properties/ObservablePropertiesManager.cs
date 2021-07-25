using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Google.Protobuf;
using OSMLSGlobalLibrary.Geometries.Observable.Actor.Property;

namespace OSMLS.Map.Features.Properties
{
	public class ObservablePropertiesManager
	{
		public ObservablePropertiesManager(Type type)
		{
			_ObservablePropertiesNamesToInfo = type.GetProperties()
				.Where(propertyInfo => propertyInfo.IsDefined(typeof(ObservablePropertyAttribute), false))
				.ToImmutableDictionary(
					propertyInfo => propertyInfo.Name,
					propertyInfo => propertyInfo
				);

			_ObservablePropertiesInfoToAttributes = _ObservablePropertiesNamesToInfo.Values
				.ToImmutableDictionary(
					propertyInfo => propertyInfo,
					propertyInfo =>
						(ObservablePropertyAttribute) propertyInfo.GetCustomAttribute(
							typeof(ObservablePropertyAttribute))
				);

			_EditableObservablePropertiesDescriptorsToInfo = _ObservablePropertiesInfoToAttributes
				.Where(propertyInfoToObservablePropertyAttribute =>
					propertyInfoToObservablePropertyAttribute.Value.Editable)
				.ToImmutableDictionary(
					propertyInfoToObservablePropertyAttribute => propertyInfoToObservablePropertyAttribute.Value.Title,
					propertyInfoToObservablePropertyAttribute => propertyInfoToObservablePropertyAttribute.Key
				);
		}

		private readonly ImmutableDictionary<string, PropertyInfo> _ObservablePropertiesNamesToInfo;

		private readonly ImmutableDictionary<PropertyInfo, ObservablePropertyAttribute>
			_ObservablePropertiesInfoToAttributes;

		private readonly ImmutableDictionary<string, PropertyInfo> _EditableObservablePropertiesDescriptorsToInfo;

		private static readonly
			IImmutableDictionary<Type, MapFeaturesMetadata.Types.ObservablePropertyMetadata.Types.ValueType>
			TypesToObservablePropertiesTypes =
				new Dictionary<Type, MapFeaturesMetadata.Types.ObservablePropertyMetadata.Types.ValueType>
				{
					{typeof(double), MapFeaturesMetadata.Types.ObservablePropertyMetadata.Types.ValueType.Double},
					{typeof(float), MapFeaturesMetadata.Types.ObservablePropertyMetadata.Types.ValueType.Float},
					{typeof(int), MapFeaturesMetadata.Types.ObservablePropertyMetadata.Types.ValueType.Int32},
					{typeof(long), MapFeaturesMetadata.Types.ObservablePropertyMetadata.Types.ValueType.Int64},
					{typeof(uint), MapFeaturesMetadata.Types.ObservablePropertyMetadata.Types.ValueType.Uint32},
					{typeof(ulong), MapFeaturesMetadata.Types.ObservablePropertyMetadata.Types.ValueType.Uint64},
					{typeof(bool), MapFeaturesMetadata.Types.ObservablePropertyMetadata.Types.ValueType.Bool},
					{typeof(string), MapFeaturesMetadata.Types.ObservablePropertyMetadata.Types.ValueType.String},
					{typeof(ByteString), MapFeaturesMetadata.Types.ObservablePropertyMetadata.Types.ValueType.Bytes}
				}.ToImmutableDictionary();

		public IEnumerable<MapFeaturesMetadata.Types.ObservablePropertyMetadata> GetAllObservablePropertiesMetadata() =>
			_ObservablePropertiesInfoToAttributes.Select(propertyInfoToAttribute =>
				{
					var (propertyInfo, attribute) = propertyInfoToAttribute;

					return new MapFeaturesMetadata.Types.ObservablePropertyMetadata
					{
						Title = attribute.Title,
						Editable = attribute.Editable,
						ValueType = TypesToObservablePropertiesTypes[propertyInfo.PropertyType]
					};
				}
			);

		public IEnumerable<ObservableProperty> GetAllObservableProperties(object obj) =>
			_ObservablePropertiesInfoToAttributes.Select(
				propertyInfoToAttribute =>
					GetObservableProperty(obj, propertyInfoToAttribute.Key, propertyInfoToAttribute.Value)
			);

		public ObservableProperty TryGetObservableProperty(object obj, string propertyName) =>
			_ObservablePropertiesNamesToInfo.TryGetValue(propertyName, out var propertyInfo)
				? GetObservableProperty(
					obj,
					propertyInfo,
					_ObservablePropertiesInfoToAttributes[propertyInfo]
				)
				: null;

		private static ObservableProperty GetObservableProperty(
			object obj,
			PropertyInfo propertyInfo,
			ObservablePropertyAttribute observablePropertyAttribute)
		{
			var observableProperty = new ObservableProperty
			{
				Title = observablePropertyAttribute.Title,
				Value = new ObservablePropertyValue()
			};

			switch (propertyInfo.GetValue(obj))
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
						$"Property {propertyInfo.Name} is of type {propertyInfo.PropertyType}, which cannot be used for observable properties.",
						nameof(obj)
					);
			}

			return observableProperty;
		}

		public void SetObservableProperty(object obj, ObservableProperty observablePropertyUpdate) =>
			SetObservableProperty(
				obj,
				_EditableObservablePropertiesDescriptorsToInfo[observablePropertyUpdate.Title],
				observablePropertyUpdate.Value
			);

		private static void SetObservableProperty(
			object obj,
			PropertyInfo propertyInfo,
			ObservablePropertyValue observablePropertyValue)
		{
			// ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
			switch (observablePropertyValue.ValueCase)
			{
				case ObservablePropertyValue.ValueOneofCase.Double:
					propertyInfo.SetValue(obj, observablePropertyValue.Double);
					break;
				case ObservablePropertyValue.ValueOneofCase.Float:
					propertyInfo.SetValue(obj, observablePropertyValue.Float);
					break;
				case ObservablePropertyValue.ValueOneofCase.Int32:
					propertyInfo.SetValue(obj, observablePropertyValue.Int32);
					break;
				case ObservablePropertyValue.ValueOneofCase.Int64:
					propertyInfo.SetValue(obj, observablePropertyValue.Int64);
					break;
				case ObservablePropertyValue.ValueOneofCase.Uint32:
					propertyInfo.SetValue(obj, observablePropertyValue.Uint32);
					break;
				case ObservablePropertyValue.ValueOneofCase.Uint64:
					propertyInfo.SetValue(obj, observablePropertyValue.Uint64);
					break;
				case ObservablePropertyValue.ValueOneofCase.Bool:
					propertyInfo.SetValue(obj, observablePropertyValue.Bool);
					break;
				case ObservablePropertyValue.ValueOneofCase.String:
					propertyInfo.SetValue(obj, observablePropertyValue.String);
					break;
				case ObservablePropertyValue.ValueOneofCase.Bytes:
					propertyInfo.SetValue(obj, observablePropertyValue.Bytes);
					break;
				case ObservablePropertyValue.ValueOneofCase.None:
					break;
			}
		}
	}
}