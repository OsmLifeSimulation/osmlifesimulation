using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.ComponentModel;
using Moq;
using NetTopologySuite.Geometries;
using NUnit.Framework;
using OSMLS.Features.Metadata;
using OSMLS.Features.Properties;
using OSMLS.Map;
using OSMLS.Model;
using OSMLS.Model.Objects;
using OSMLS.Tests.Mocks;
using OSMLS.Types.Model;
using OSMLSGlobalLibrary.Modules;
using OSMLSGlobalLibrary.Observable.Geometries.Actor;

namespace OSMLS.Tests.Features.Properties
{
	public class MapFeaturesObservablePropertiesProviderTest
	{
		// ReSharper disable once MemberCanBePrivate.Global
		public abstract class TestGeometryActor : Point, IActor
		{
			// ReSharper disable once PublicConstructorInAbstractClass
			public TestGeometryActor() : base(new Coordinate())
			{
			}

			public event EventHandler CoordinatesChanged
			{
				add => throw new NotSupportedException();
				remove => throw new NotSupportedException();
			}

			public virtual event PropertyChangedEventHandler PropertyChanged
			{
				add => throw new NotSupportedException();
				remove => throw new NotSupportedException();
			}

			// ReSharper disable once UnassignedGetOnlyAutoProperty
			public virtual Guid Id { get; }
		}

		private static Mock<IObservablePropertiesManager> ComposeObservablePropertiesManagerMockForGetting(
			object keyObject,
			ObservableProperty observableProperty)
		{
			var observablePropertiesManagerMock = new Mock<IObservablePropertiesManager>();
			observablePropertiesManagerMock
				.Setup(observablePropertiesManager =>
					observablePropertiesManager.GetAllObservableProperties(keyObject)
				).Returns(new[] { observableProperty });

			return observablePropertiesManagerMock;
		}

		private static Mock<IObservablePropertiesManager> ComposeObservablePropertiesManagerMockForGetting(
			object keyObject,
			string observablePropertyName,
			string notObservablePropertyName,
			ObservableProperty observableProperty)
		{
			var observablePropertiesManagerMock = new Mock<IObservablePropertiesManager>();
			observablePropertiesManagerMock
				.Setup(observablePropertiesManager =>
					observablePropertiesManager.IsPropertyObservable(observablePropertyName)
				).Returns(true);
			observablePropertiesManagerMock
				.Setup(observablePropertiesManager =>
					observablePropertiesManager.IsPropertyObservable(notObservablePropertyName)
				).Returns(false);
			observablePropertiesManagerMock
				.Setup(observablePropertiesManager =>
					observablePropertiesManager.GetObservableProperty(keyObject, observablePropertyName)
				).Returns(observableProperty);

			return observablePropertiesManagerMock;
		}

		private static Mock<IObservablePropertiesManager> ComposeObservablePropertiesManagerMockForSetting()
		{
			var observablePropertiesManagerMock = new Mock<IObservablePropertiesManager>();
			observablePropertiesManagerMock
				.Setup(observablePropertiesManager =>
					observablePropertiesManager
						.SetObservableProperty(It.IsAny<object>(), It.IsAny<ObservableProperty>())
				).Verifiable();

			return observablePropertiesManagerMock;
		}

		private static Mock<IObservablePropertiesMetadataProvider> ComposeObservablePropertiesMetadataProviderMock(
			Type type,
			IObservablePropertiesManager observablePropertiesManager)
		{
			var observablePropertiesMetadataProviderMock = new Mock<IObservablePropertiesMetadataProvider>();
			observablePropertiesMetadataProviderMock
				.Setup(observablePropertiesMetadataProvider =>
					observablePropertiesMetadataProvider
						.TypesToObservablePropertiesManagers[type]
				).Returns(observablePropertiesManager);

			return observablePropertiesMetadataProviderMock;
		}

		private static Mock<IModelProvider> ComposeEmptyModelServiceMock()
		{
			var modelServiceMock = new Mock<IModelProvider>();
			modelServiceMock
				.Setup(modelService => modelService.TypesToInitializedModules)
				.Returns(ImmutableDictionary.Create<IInjectedModuleType, IModule>());

			return modelServiceMock;
		}

		private static Mock<IModelProvider> ComposeModelServiceMock(
			IInjectedModuleType moduleType,
			IModule module)
		{
			var modelServiceMock = new Mock<IModelProvider>();
			modelServiceMock
				.Setup(modelService => modelService.TypesToInitializedModules)
				.Returns(
					ImmutableDictionary.Create<IInjectedModuleType, IModule>()
						.Add(moduleType, module)
				);

			return modelServiceMock;
		}

		[Test]
		public void ShouldProvideMapFeaturesObservablePropertiesObservableForModulesProperly()
		{
			var moduleMock = new Mock<IModule>();
			var moduleType = moduleMock.Object.GetType();

			var observableProperty = new ObservableProperty();

			var modelServiceMock = new Mock<IModelProvider>();

			var mapFeaturesObservablePropertiesProvider = new MapFeaturesObservablePropertiesProvider(
				ComposeObservablePropertiesMetadataProviderMock(
					moduleType,
					ComposeObservablePropertiesManagerMockForGetting(moduleMock.Object, observableProperty).Object
				).Object,
				modelServiceMock.Object,
				new Mock<IMapObjectsCollection>().Object
			);

			var actualMapFeaturesObservableProperties = new List<MapFeatureObservableProperty>();
			var mapFeaturesObservablePropertiesObservableSubscription = mapFeaturesObservablePropertiesProvider
				.MapFeaturesObservablePropertiesObservable
				.Subscribe(property => actualMapFeaturesObservableProperties.Add(property));

			using (mapFeaturesObservablePropertiesObservableSubscription)
				modelServiceMock.Raise(
					modelService => modelService.ModuleInitialized += null,
					new INotifyModuleInitialized.ModuleInitializedEventArgs(moduleMock.Object)
				);

			Assert.That(actualMapFeaturesObservableProperties, Has.Count.EqualTo(1));
			Assert.That(actualMapFeaturesObservableProperties, Has.One.Matches<MapFeatureObservableProperty>(
				mapFeatureObservableProperty =>
					mapFeatureObservableProperty.TypeFullName == moduleType.FullName &&
					mapFeatureObservableProperty.Id == moduleType.FullName &&
					mapFeatureObservableProperty.ObservableProperty.Equals(observableProperty)
			));
		}

		[Test]
		public void ShouldProvideMapFeaturesObservablePropertiesObservableForModulesObservablePropertiesProperly()
		{
			var moduleMock = new Mock<IModule>();
			var moduleType = moduleMock.Object.GetType();

			var observablePropertyName = Guid.NewGuid().ToString();
			var notObservablePropertyName = Guid.NewGuid().ToString();

			var observableProperty = new ObservableProperty();

			var modelServiceMock = new Mock<IModelProvider>();

			var mapFeaturesObservablePropertiesProvider = new MapFeaturesObservablePropertiesProvider(
				ComposeObservablePropertiesMetadataProviderMock(
					moduleType,
					ComposeObservablePropertiesManagerMockForGetting(
						moduleMock.Object,
						observablePropertyName,
						notObservablePropertyName,
						observableProperty
					).Object
				).Object,
				modelServiceMock.Object,
				new Mock<IMapObjectsCollection>().Object
			);

			var actualMapFeaturesObservableProperties = new List<MapFeatureObservableProperty>();
			var mapFeaturesObservablePropertiesObservableSubscription = mapFeaturesObservablePropertiesProvider
				.MapFeaturesObservablePropertiesObservable
				.Subscribe(property => actualMapFeaturesObservableProperties.Add(property));

			using (mapFeaturesObservablePropertiesObservableSubscription)
			{
				modelServiceMock.Raise(
					modelService => modelService.ModuleInitialized += null,
					new INotifyModuleInitialized.ModuleInitializedEventArgs(moduleMock.Object)
				);

				moduleMock.Raise(
					module => module.PropertyChanged += null,
					new PropertyChangedEventArgs(observablePropertyName)
				);

				moduleMock.Raise(
					module => module.PropertyChanged += null,
					new PropertyChangedEventArgs(notObservablePropertyName)
				);
			}

			Assert.That(actualMapFeaturesObservableProperties, Has.Count.EqualTo(1));
			Assert.That(actualMapFeaturesObservableProperties, Has.One.Matches<MapFeatureObservableProperty>(
				mapFeatureObservableProperty =>
					mapFeatureObservableProperty.TypeFullName == moduleType.FullName &&
					mapFeatureObservableProperty.Id == moduleType.FullName &&
					mapFeatureObservableProperty.ObservableProperty.Equals(observableProperty)
			));
		}

		[Test]
		public void ShouldThrowArgumentOutOfRangeExceptionOnMapObjectsCollectionChangedWrongAction()
		{
			var mapObjectsCollectionMock = new Mock<IMapObjectsCollection>();

			var _ = new MapFeaturesObservablePropertiesProvider(
				new Mock<IObservablePropertiesMetadataProvider>().Object,
				new Mock<IModelProvider>().Object,
				mapObjectsCollectionMock.Object
			);

			Assert.Throws<ArgumentOutOfRangeException>(() =>
				mapObjectsCollectionMock.Raise(
					mapObjectsCollection => mapObjectsCollection.CollectionChanged += null,
					new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset)
				)
			);
		}

		[Test]
		public void ShouldProvideMapFeaturesObservablePropertiesObservableForGeometryActorsProperly()
		{
			var geometryActorMock = new Mock<TestGeometryActor>();
			geometryActorMock
				.Setup(geometryActor => geometryActor.Id)
				.Returns(Guid.NewGuid());

			var geometryActorMockType = geometryActorMock.Object.GetType();

			var observableProperty = new ObservableProperty();

			var mapObjectsCollectionMock = new Mock<IMapObjectsCollection>();

			var mapFeaturesObservablePropertiesProvider = new MapFeaturesObservablePropertiesProvider(
				ComposeObservablePropertiesMetadataProviderMock(
					geometryActorMockType,
					ComposeObservablePropertiesManagerMockForGetting(
						geometryActorMock.Object,
						observableProperty
					).Object
				).Object,
				new Mock<IModelProvider>().Object,
				mapObjectsCollectionMock.Object
			);

			var actualMapFeaturesObservableProperties = new List<MapFeatureObservableProperty>();
			var mapFeaturesObservablePropertiesObservableSubscription = mapFeaturesObservablePropertiesProvider
				.MapFeaturesObservablePropertiesObservable
				.Subscribe(property => actualMapFeaturesObservableProperties.Add(property));

			using (mapFeaturesObservablePropertiesObservableSubscription)
				mapObjectsCollectionMock.Raise(
					mapObjectsCollection => mapObjectsCollection.CollectionChanged += null,
					new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, geometryActorMock.Object)
				);

			Assert.That(actualMapFeaturesObservableProperties, Has.Count.EqualTo(1));
			Assert.That(actualMapFeaturesObservableProperties, Has.One.Matches<MapFeatureObservableProperty>(
				mapFeatureObservableProperty =>
					mapFeatureObservableProperty.TypeFullName == geometryActorMockType.FullName &&
					mapFeatureObservableProperty.Id == geometryActorMock.Object.Id.ToString() &&
					mapFeatureObservableProperty.ObservableProperty.Equals(observableProperty)
			));
		}

		[Test]
		public void
			ShouldProvideMapFeaturesObservablePropertiesObservableForGeometryActorsObservablePropertiesProperly()
		{
			var geometryActorMock = new Mock<TestGeometryActor>();
			geometryActorMock
				.Setup(geometryActor => geometryActor.Id)
				.Returns(Guid.NewGuid());

			var geometryActorMockType = geometryActorMock.Object.GetType();

			var observablePropertyName = Guid.NewGuid().ToString();
			var notObservablePropertyName = Guid.NewGuid().ToString();

			var observableProperty = new ObservableProperty();

			var mapObjectsCollectionMock = new Mock<IMapObjectsCollection>();

			var mapFeaturesObservablePropertiesProvider = new MapFeaturesObservablePropertiesProvider(
				ComposeObservablePropertiesMetadataProviderMock(
					geometryActorMockType,
					ComposeObservablePropertiesManagerMockForGetting(
						geometryActorMock.Object,
						observablePropertyName,
						notObservablePropertyName,
						observableProperty
					).Object
				).Object,
				new Mock<IModelProvider>().Object,
				mapObjectsCollectionMock.Object
			);

			var actualMapFeaturesObservableProperties = new List<MapFeatureObservableProperty>();
			var mapFeaturesObservablePropertiesObservableSubscription = mapFeaturesObservablePropertiesProvider
				.MapFeaturesObservablePropertiesObservable
				.Subscribe(property => actualMapFeaturesObservableProperties.Add(property));

			using (mapFeaturesObservablePropertiesObservableSubscription)
			{
				mapObjectsCollectionMock.Raise(
					mapObjectsCollection => mapObjectsCollection.CollectionChanged += null,
					new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, geometryActorMock.Object)
				);

				geometryActorMock.Raise(
					geometryActor => geometryActor.PropertyChanged += null,
					new PropertyChangedEventArgs(observablePropertyName)
				);

				geometryActorMock.Raise(
					geometryActor => geometryActor.PropertyChanged += null,
					new PropertyChangedEventArgs(notObservablePropertyName)
				);

				Assert.That(actualMapFeaturesObservableProperties, Has.Count.EqualTo(1));
				Assert.That(actualMapFeaturesObservableProperties, Has.One.Matches<MapFeatureObservableProperty>(
					mapFeatureObservableProperty =>
						mapFeatureObservableProperty.TypeFullName == geometryActorMockType.FullName &&
						mapFeatureObservableProperty.Id == geometryActorMock.Object.Id.ToString() &&
						mapFeatureObservableProperty.ObservableProperty.Equals(observableProperty)
				));

				// Should be able to unsubscribe.

				mapObjectsCollectionMock.Raise(
					mapObjectsCollection => mapObjectsCollection.CollectionChanged += null,
					new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, geometryActorMock.Object)
				);

				geometryActorMock.Raise(
					geometryActor => geometryActor.PropertyChanged += null,
					new PropertyChangedEventArgs(observablePropertyName)
				);

				Assert.That(actualMapFeaturesObservableProperties, Has.Count.EqualTo(1));
			}
		}

		[Test]
		public void ShouldGetMapFeaturesObservablePropertiesForModulesProperly()
		{
			var moduleTypeMock = MocksComposer.ComposeInjectedTypeMock<IInjectedModuleType>();
			var moduleMock = new Mock<IModule>();

			var observableProperty = new ObservableProperty();

			var mapFeaturesObservablePropertiesProvider = new MapFeaturesObservablePropertiesProvider(
				ComposeObservablePropertiesMetadataProviderMock(
					moduleTypeMock.Object.SystemType,
					ComposeObservablePropertiesManagerMockForGetting(moduleMock.Object, observableProperty).Object
				).Object,
				ComposeModelServiceMock(moduleTypeMock.Object, moduleMock.Object).Object,
				MocksComposer.ComposeEmptyMapObjectsCollectionMock().Object
			);

			var actualMapFeaturesObservableProperties =
				mapFeaturesObservablePropertiesProvider.GetMapFeaturesObservableProperties().ToImmutableList();

			Assert.That(actualMapFeaturesObservableProperties, Has.Count.EqualTo(1));
			Assert.That(
				actualMapFeaturesObservableProperties,
				Has.One.Matches<MapFeatureObservableProperty>(mapFeatureObservableProperty =>
					mapFeatureObservableProperty.TypeFullName == moduleTypeMock.Object.FullName &&
					mapFeatureObservableProperty.Id == moduleTypeMock.Object.FullName &&
					mapFeatureObservableProperty.ObservableProperty.Equals(observableProperty)
				)
			);
		}

		[Test]
		public void ShouldGetMapFeaturesObservablePropertiesForGeometryActorsProperly()
		{
			var geometryActorMock = new Mock<TestGeometryActor>();
			geometryActorMock
				.Setup(geometryActor => geometryActor.Id)
				.Returns(Guid.NewGuid());

			var geometryActorMockType = geometryActorMock.Object.GetType();

			var observableProperty = new ObservableProperty();

			var mapFeaturesObservablePropertiesProvider = new MapFeaturesObservablePropertiesProvider(
				ComposeObservablePropertiesMetadataProviderMock(
					geometryActorMockType,
					ComposeObservablePropertiesManagerMockForGetting(
						geometryActorMock.Object,
						observableProperty
					).Object
				).Object,
				ComposeEmptyModelServiceMock().Object,
				MocksComposer.ComposeMapObjectsCollectionMock(geometryActorMock.Object).Object
			);

			var actualMapFeaturesObservableProperties =
				mapFeaturesObservablePropertiesProvider.GetMapFeaturesObservableProperties().ToImmutableList();

			Assert.That(actualMapFeaturesObservableProperties, Has.Count.EqualTo(1));
			Assert.That(
				actualMapFeaturesObservableProperties,
				Has.One.Matches<MapFeatureObservableProperty>(mapFeatureObservableProperty =>
					mapFeatureObservableProperty.TypeFullName == geometryActorMockType.FullName &&
					mapFeatureObservableProperty.Id == geometryActorMock.Object.Id.ToString() &&
					mapFeatureObservableProperty.ObservableProperty.Equals(observableProperty)
				)
			);
		}

		[Test]
		public void ShouldSetMapFeaturesObservablePropertiesForModulesProperly()
		{
			var moduleTypeMock = MocksComposer.ComposeInjectedTypeMock<IInjectedModuleType>();
			var moduleMock = new Mock<IModule>();

			var mapFeatureObservableProperty = new MapFeatureObservableProperty
			{
				TypeFullName = moduleTypeMock.Object.FullName,
				Id = moduleTypeMock.Object.FullName,
				ObservableProperty = new ObservableProperty()
			};

			var observablePropertiesManagerMock = ComposeObservablePropertiesManagerMockForSetting();

			new MapFeaturesObservablePropertiesProvider(
				ComposeObservablePropertiesMetadataProviderMock(
					moduleTypeMock.Object.SystemType,
					observablePropertiesManagerMock.Object
				).Object,
				ComposeModelServiceMock(moduleTypeMock.Object, moduleMock.Object).Object,
				new Mock<IMapObjectsCollection>().Object
			).SetMapFeatureObservableProperty(mapFeatureObservableProperty);

			observablePropertiesManagerMock.Verify(
				observablePropertiesManager => observablePropertiesManager
					.SetObservableProperty(moduleMock.Object, mapFeatureObservableProperty.ObservableProperty),
				Times.Once
			);
		}

		[Test]
		public void ShouldSetMapFeaturesObservablePropertiesForGeometryActorsProperly()
		{
			var geometryActorMock = new Mock<TestGeometryActor>();
			geometryActorMock
				.Setup(geometryActor => geometryActor.Id)
				.Returns(Guid.NewGuid());

			var geometryActorMockType = geometryActorMock.Object.GetType();

			var mapFeatureObservableProperty = new MapFeatureObservableProperty
			{
				TypeFullName = Guid.NewGuid().ToString(),
				Id = geometryActorMock.Object.Id.ToString(),
				ObservableProperty = new ObservableProperty()
			};

			var observablePropertiesManagerMock = ComposeObservablePropertiesManagerMockForSetting();

			new MapFeaturesObservablePropertiesProvider(
				ComposeObservablePropertiesMetadataProviderMock(
					geometryActorMockType,
					observablePropertiesManagerMock.Object
				).Object,
				new Mock<IModelProvider>().Object,
				MocksComposer.ComposeMapObjectsCollectionMock(geometryActorMock.Object).Object
			).SetMapFeatureObservableProperty(mapFeatureObservableProperty);

			observablePropertiesManagerMock.Verify(
				observablePropertiesManager => observablePropertiesManager.SetObservableProperty(
					geometryActorMock.Object,
					mapFeatureObservableProperty.ObservableProperty
				),
				Times.Once
			);
		}
	}
}