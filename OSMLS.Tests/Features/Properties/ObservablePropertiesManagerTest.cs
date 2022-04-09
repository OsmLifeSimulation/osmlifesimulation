using System;
using System.Linq;
using Google.Protobuf;
using Moq;
using MoreLinq;
using NUnit.Framework;
using OSMLS.Features.Properties;
using OSMLS.Map;
using OSMLS.Types.Model.Properties;
using OSMLSGlobalLibrary.Observable.Property;

namespace OSMLS.Tests.Features.Properties
{
	public class ObservablePropertiesManagerTest
	{
		private static Mock<IInjectedProperty> ComposeObservablePropertyMock(
			object objectWithObservableProperty = null,
			object observablePropertyValue = null,
			bool isEditable = false)
		{
			var observablePropertyName = Guid.NewGuid().ToString();
			var observablePropertyAttribute = new ObservablePropertyAttribute(observablePropertyName, isEditable);
			var observablePropertyMock = new Mock<IInjectedProperty>();
			observablePropertyMock
				.Setup(observableProperty => observableProperty.IsObservable)
				.Returns(true);
			observablePropertyMock
				.Setup(observableProperty => observableProperty.Name)
				.Returns(observablePropertyName);
			observablePropertyMock
				.Setup(observableProperty => observableProperty.ObservablePropertyAttribute)
				.Returns(observablePropertyAttribute);

			if (objectWithObservableProperty != null)
				observablePropertyMock.Setup(observableProperty =>
					observableProperty.SetValue(objectWithObservableProperty, It.IsAny<object>())
				).Verifiable();

			if (observablePropertyValue != null)
				observablePropertyMock
					.Setup(observableProperty => observableProperty.PropertyType)
					.Returns(observablePropertyValue.GetType());

			if (objectWithObservableProperty != null && observablePropertyValue != null)
				observablePropertyMock
					.Setup(observableProperty => observableProperty.GetValue(objectWithObservableProperty))
					.Returns(observablePropertyValue);

			return observablePropertyMock;
		}

		[Test]
		public void ShouldGetAllObservablePropertiesMetadataProperly()
		{
			var observablePropertyMock =
				ComposeObservablePropertyMock(observablePropertyValue: Guid.NewGuid().ToString());
			var observablePropertyAttribute = observablePropertyMock.Object.ObservablePropertyAttribute;

			var notObservablePropertyMock = new Mock<IInjectedProperty>();
			notObservablePropertyMock
				.Setup(notObservableProperty => notObservableProperty.IsObservable)
				.Returns(false);

			var actualObservablePropertiesMetadata = new ObservablePropertiesManager(new[]
			{
				observablePropertyMock.Object,
				notObservablePropertyMock.Object
			}).GetAllObservablePropertiesMetadata().ToList();

			Assert.That(actualObservablePropertiesMetadata, Has.Count.EqualTo(1));
			Assert.That(actualObservablePropertiesMetadata,
				Has.One.Matches<MapFeaturesMetadata.Types.ObservablePropertyMetadata>(
					observablePropertyMetadata =>
						observablePropertyMetadata.Title == observablePropertyAttribute.Title &&
						observablePropertyMetadata.Editable == observablePropertyAttribute.Editable &&
						observablePropertyMetadata.ValueType
						== MapFeaturesMetadata.Types.ObservablePropertyMetadata.Types.ValueType.String
				));
		}

		[Test]
		public void ShouldGetAllObservablePropertiesProperly()
		{
			var objectWithObservableProperty = new object();
			var observablePropertyValue = Guid.NewGuid().ToString();
			var observablePropertyMock = ComposeObservablePropertyMock(
				objectWithObservableProperty,
				observablePropertyValue
			);
			var observablePropertyAttribute = observablePropertyMock.Object.ObservablePropertyAttribute;

			var notObservablePropertyMock = new Mock<IInjectedProperty>();
			notObservablePropertyMock
				.Setup(notObservableProperty => notObservableProperty.IsObservable)
				.Returns(false);

			var actualObservableProperties = new ObservablePropertiesManager(new[]
			{
				observablePropertyMock.Object,
				notObservablePropertyMock.Object
			}).GetAllObservableProperties(objectWithObservableProperty).ToList();

			Assert.That(actualObservableProperties, Has.Count.EqualTo(1));
			Assert.That(actualObservableProperties, Has.One.Matches<ObservableProperty>(observablePropertyMetadata =>
				observablePropertyMetadata.Title == observablePropertyAttribute.Title &&
				observablePropertyMetadata.Value.String == observablePropertyValue
			));
		}

		[Test]
		public void ShouldCheckIfPropertyIsObservableProperly()
		{
			var observablePropertyMock = ComposeObservablePropertyMock();

			var notObservablePropertyName = Guid.NewGuid().ToString();
			var notObservablePropertyMock = new Mock<IInjectedProperty>();
			notObservablePropertyMock
				.Setup(observableProperty => observableProperty.Name)
				.Returns(notObservablePropertyName);
			notObservablePropertyMock
				.Setup(notObservableProperty => notObservableProperty.IsObservable)
				.Returns(false);

			var observablePropertiesManager = new ObservablePropertiesManager(new[]
			{
				observablePropertyMock.Object,
				notObservablePropertyMock.Object
			});

			Assert.IsTrue(observablePropertiesManager.IsPropertyObservable(observablePropertyMock.Object.Name));
			Assert.IsFalse(observablePropertiesManager.IsPropertyObservable(notObservablePropertyName));
		}

		private static ObservablePropertyValue ComposeObservablePropertyValue(object value)
		{
			var observablePropertyValue = new ObservablePropertyValue();
			switch (value)
			{
				case double doubleValue:
					observablePropertyValue.Double = doubleValue;
					break;
				case float floatValue:
					observablePropertyValue.Float = floatValue;
					break;
				case int intValue:
					observablePropertyValue.Int32 = intValue;
					break;
				case long longValue:
					observablePropertyValue.Int64 = longValue;
					break;
				case uint uintValue:
					observablePropertyValue.Uint32 = uintValue;
					break;
				case ulong ulongValue:
					observablePropertyValue.Uint64 = ulongValue;
					break;
				case bool boolValue:
					observablePropertyValue.Bool = boolValue;
					break;
				case string stringValue:
					observablePropertyValue.String = stringValue;
					break;
				case ByteString byteStringValue:
					observablePropertyValue.Bytes = byteStringValue;
					break;
			}

			return observablePropertyValue;
		}

		[Test]
		public void ShouldGetObservablePropertyProperly()
		{
			new object[]
			{
				0.1,
				0.1f,
				1,
				(long)1,
				(uint)1,
				(ulong)1,
				true,
				"",
				ByteString.Empty
			}.ForEach(observablePropertyValue =>
			{
				var objectWithObservableProperty = new object();
				var observablePropertyMock = ComposeObservablePropertyMock(
					objectWithObservableProperty,
					observablePropertyValue
				);

				var observablePropertiesManager =
					new ObservablePropertiesManager(new[] { observablePropertyMock.Object });

				Assert.That(
					observablePropertiesManager.GetObservableProperty(
						objectWithObservableProperty,
						observablePropertyMock.Object.Name
					),
					Is.EqualTo(new ObservableProperty
					{
						Title = observablePropertyMock.Object.ObservablePropertyAttribute.Title,
						Value = ComposeObservablePropertyValue(observablePropertyValue)
					})
				);
			});
		}

		private class TestUnsupportedPropertyType
		{
		}

		[Test]
		public void ShouldThrowArgumentExceptionOnTryingToGetObservablePropertyOfUnsupportedType()
		{
			var observablePropertyValue = new TestUnsupportedPropertyType();
			var objectWithObservableProperty = new object();
			var observablePropertyMock = ComposeObservablePropertyMock(
				objectWithObservableProperty,
				observablePropertyValue
			);

			var observablePropertiesManager =
				new ObservablePropertiesManager(new[] { observablePropertyMock.Object });

			Assert.Throws<ArgumentException>(() =>
				observablePropertiesManager.GetObservableProperty(
					objectWithObservableProperty,
					observablePropertyMock.Object.Name
				)
			);
		}

		[Test]
		public void ShouldSetObservablePropertyProperly()
		{
			new object[]
			{
				0.1,
				0.1f,
				1,
				(long)1,
				(uint)1,
				(ulong)1,
				true,
				"",
				ByteString.Empty
			}.ForEach(updatedObservablePropertyValue =>
			{
				var objectWithObservableProperty = new object();
				var observablePropertyMock =
					ComposeObservablePropertyMock(objectWithObservableProperty, isEditable: true);

				new ObservablePropertiesManager(new[] { observablePropertyMock.Object })
					.SetObservableProperty(objectWithObservableProperty, new ObservableProperty
						{
							Title = observablePropertyMock.Object.Name,
							Value = ComposeObservablePropertyValue(updatedObservablePropertyValue)
						}
					);

				observablePropertyMock.Verify(
					observableProperty =>
						observableProperty.SetValue(objectWithObservableProperty, updatedObservablePropertyValue),
					Times.Once
				);
			});
		}

		[Test]
		public void ShouldNotSetObservablePropertyOfUnsupportedType()
		{
			var updatedObservablePropertyValue = new TestUnsupportedPropertyType();
			var objectWithObservableProperty = new object();
			var observablePropertyMock =
				ComposeObservablePropertyMock(objectWithObservableProperty, isEditable: true);

			new ObservablePropertiesManager(new[] { observablePropertyMock.Object })
				.SetObservableProperty(objectWithObservableProperty, new ObservableProperty
					{
						Title = observablePropertyMock.Object.Name,
						Value = ComposeObservablePropertyValue(updatedObservablePropertyValue)
					}
				);

			observablePropertyMock.Verify(
				observableProperty =>
					observableProperty.SetValue(objectWithObservableProperty, updatedObservablePropertyValue),
				Times.Never
			);
		}
	}
}