using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using NUnit.Framework;
using OSMLS.Features.Metadata;
using OSMLS.Map;
using OSMLS.Tests.Mocks;
using OSMLS.Types;
using OSMLS.Types.Model;

namespace OSMLS.Tests.Features.Metadata
{
	public class MapMetadataProviderTest
	{
		[Test]
		public void ShouldComposeTypesToObservablePropertiesManagersFromTypesProviderProperly()
		{
			var testTypes = Enumerable.Range(0, 5)
				.Select(_ => MocksComposer.ComposeInjectedTypeMock<IInjectedType>().Object)
				.ToImmutableList();

			var mapMetadataProvider = new MapMetadataProvider(
				MocksComposer.ComposeTypesProviderMock(testTypes).Object
			);

			Assert.That(
				mapMetadataProvider.TypesToObservablePropertiesManagers.Keys,
				Is.EqualTo(testTypes.Select(type => type.SystemType))
			);
		}

		[Test]
		public void ShouldComposeTypesToObservablePropertiesManagersFromTypesProviderEventsProperly()
		{
			var testTypes = Enumerable.Range(0, 5)
				.Select(_ => MocksComposer.ComposeInjectedTypeMock<IInjectedType>().Object)
				.ToImmutableList();

			var typesProviderMock = MocksComposer.ComposeTypesProviderMock(Array.Empty<IInjectedType>());

			var mapMetadataProvider = new MapMetadataProvider(typesProviderMock.Object);

			testTypes.ForEach(type =>
				typesProviderMock.Raise(typesProvider =>
						typesProvider.TypeAdded += null, new IInjectedTypesProvider.TypeAddedEventArgs(type)
				)
			);

			Assert.That(
				mapMetadataProvider.TypesToObservablePropertiesManagers.Keys,
				Is.EqualTo(testTypes.Select(type => type.SystemType))
			);
		}

		[Test]
		public void ShouldProvideMapFeaturesMetadataObservableProperly()
		{
			var typeMock = MocksComposer.ComposeInjectedTypeMock<IInjectedModuleType>();

			var typesProviderMock = MocksComposer.ComposeTypesProviderMock(Array.Empty<IInjectedType>());

			var mapMetadataProvider = new MapMetadataProvider(typesProviderMock.Object);

			var actualMapFeaturesMetadata = new List<MapFeaturesMetadata>();

			var mapFeaturesMetadataObservableSubscription = mapMetadataProvider.MapFeaturesMetadataObservable
				.Subscribe(metadata => actualMapFeaturesMetadata.Add(metadata));

			using (mapFeaturesMetadataObservableSubscription)
				typesProviderMock.Raise(typesProvider =>
					typesProvider.TypeAdded += null, new IInjectedTypesProvider.TypeAddedEventArgs(typeMock.Object));

			Assert.That(actualMapFeaturesMetadata, Has.Count.EqualTo(1));
			Assert.That(actualMapFeaturesMetadata, Has.One.Matches<MapFeaturesMetadata>(mapFeaturesMetadata =>
				mapFeaturesMetadata.TypeFullName == typeMock.Object.FullName &&
				mapFeaturesMetadata.OpenLayersStyle == string.Empty
			));
		}

		[Test]
		public void ShouldNotComposeMapFeaturesMetadataFromNonModuleAndNonActorType()
		{
			var typeMock = MocksComposer.ComposeInjectedTypeMock<IInjectedType>();

			var mapMetadataProvider = new MapMetadataProvider(
				MocksComposer.ComposeTypesProviderMock(typeMock.Object).Object
			);

			var actualMapFeaturesMetadata = mapMetadataProvider.GetMapFeaturesMetadata().ToList();

			Assert.That(actualMapFeaturesMetadata, Has.Count.EqualTo(0));
		}

		[Test]
		public void ShouldComposeMapFeaturesMetadataFromModuleTypeProperly()
		{
			var typeMock = MocksComposer.ComposeInjectedTypeMock<IInjectedModuleType>();

			var actualMapFeaturesMetadata = new MapMetadataProvider(
				MocksComposer.ComposeTypesProviderMock(typeMock.Object).Object
			).GetMapFeaturesMetadata().ToList();

			Assert.That(actualMapFeaturesMetadata, Has.Count.EqualTo(1));
			Assert.That(actualMapFeaturesMetadata, Has.One.Matches<MapFeaturesMetadata>(mapFeaturesMetadata =>
				mapFeaturesMetadata.TypeFullName == typeMock.Object.FullName &&
				mapFeaturesMetadata.OpenLayersStyle == string.Empty
			));
		}

		[Test]
		public void ShouldComposeMapFeaturesMetadataFromActorTypeProperly()
		{
			var testCustomStyleAttribute = Guid.NewGuid().ToString();

			var typeMock = MocksComposer.ComposeInjectedTypeMock<IInjectedActorType>();
			typeMock
				.Setup(type => type.IsVisible)
				.Returns(true);
			typeMock
				.Setup(type => type.GetCustomStyle())
				.Returns(testCustomStyleAttribute);

			var actualMapFeaturesMetadata = new MapMetadataProvider(
				MocksComposer.ComposeTypesProviderMock(typeMock.Object).Object
			).GetMapFeaturesMetadata().ToList();

			Assert.That(actualMapFeaturesMetadata, Has.Count.EqualTo(1));
			Assert.That(actualMapFeaturesMetadata, Has.One.Matches<MapFeaturesMetadata>(mapFeaturesMetadata =>
				mapFeaturesMetadata.TypeFullName == typeMock.Object.FullName &&
				mapFeaturesMetadata.OpenLayersStyle == testCustomStyleAttribute
			));
		}
	}
}