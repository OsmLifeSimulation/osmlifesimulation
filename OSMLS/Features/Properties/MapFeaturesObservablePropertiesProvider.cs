using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Subjects;
using NetTopologySuite.Geometries;
using OSMLS.Features.Metadata;
using OSMLS.Map;
using OSMLS.Model;
using OSMLS.Model.Objects;
using OSMLSGlobalLibrary.Observable.Geometries.Actor;

namespace OSMLS.Features.Properties
{
	public class MapFeaturesObservablePropertiesProvider : IMapFeaturesObservablePropertiesProvider
	{
		public MapFeaturesObservablePropertiesProvider(
			IObservablePropertiesMetadataProvider observablePropertiesMetadataProvider,
			IModelProvider modelProvider,
			IMapObjectsCollection mapObjectsCollection)
		{
			_ObservablePropertiesMetadataProvider = observablePropertiesMetadataProvider;
			_ModelProvider = modelProvider;
			_MapObjectsCollection = mapObjectsCollection;
			var mapFeaturesObservablePropertiesSubject = new Subject<MapFeatureObservableProperty>();
			MapFeaturesObservablePropertiesObservable = mapFeaturesObservablePropertiesSubject;

			_ModelProvider.ModuleInitialized += (_, args) =>
			{
				void PropertyChangedEventHandler(object obj, PropertyChangedEventArgs eventArgs)
				{
					var type = obj.GetType();
					var observablePropertiesManager = _ObservablePropertiesMetadataProvider
						.TypesToObservablePropertiesManagers[type];

					if (!observablePropertiesManager.IsPropertyObservable(eventArgs.PropertyName))
						return;

					mapFeaturesObservablePropertiesSubject.OnNext(
						new MapFeatureObservableProperty
						{
							TypeFullName = type.FullName,
							Id = type.FullName,
							ObservableProperty = observablePropertiesManager
								.GetObservableProperty(obj, eventArgs.PropertyName)
						}
					);
				}

				var moduleType = args.Module.GetType();
				_ObservablePropertiesMetadataProvider
					.TypesToObservablePropertiesManagers[moduleType]
					.GetAllObservableProperties(args.Module)
					.ToList()
					.ForEach(property => mapFeaturesObservablePropertiesSubject.OnNext(
						new MapFeatureObservableProperty
						{
							TypeFullName = moduleType.FullName,
							Id = moduleType.FullName,
							ObservableProperty = property
						}
					));

				args.Module.PropertyChanged += PropertyChangedEventHandler;
			};

			_MapObjectsCollection.CollectionChanged += (_, args) =>
			{
				void PropertyChangedEventHandler(object obj, PropertyChangedEventArgs eventArgs)
				{
					var type = obj.GetType();
					var observablePropertiesManager = _ObservablePropertiesMetadataProvider
						.TypesToObservablePropertiesManagers[type];

					if (!observablePropertiesManager.IsPropertyObservable(eventArgs.PropertyName))
						return;

					mapFeaturesObservablePropertiesSubject.OnNext(
						new MapFeatureObservableProperty
						{
							TypeFullName = type.FullName,
							Id = ((IActor)obj).Id.ToString(),
							ObservableProperty = observablePropertiesManager
								.GetObservableProperty(obj, eventArgs.PropertyName)
						}
					);
				}

				// ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
				switch (args.Action)
				{
					case NotifyCollectionChangedAction.Add:
						foreach (var actor in args.NewItems!.OfType<IActor>())
						{
							actor.PropertyChanged += PropertyChangedEventHandler;

							var type = actor.GetType();
							_ObservablePropertiesMetadataProvider
								.TypesToObservablePropertiesManagers[type]
								.GetAllObservableProperties(actor)
								.Select(observableProperty =>
									new MapFeatureObservableProperty
									{
										TypeFullName = type.FullName,
										Id = actor.Id.ToString(),
										ObservableProperty = observableProperty
									})
								.ToList()
								.ForEach(mapFeatureObservableProperty =>
									mapFeaturesObservablePropertiesSubject.OnNext(mapFeatureObservableProperty)
								);
						}

						break;
					case NotifyCollectionChangedAction.Remove:
						foreach (var actor in args.OldItems!.OfType<IActor>())
						{
							actor.PropertyChanged -= PropertyChangedEventHandler;
						}

						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			};
		}

		private readonly IObservablePropertiesMetadataProvider _ObservablePropertiesMetadataProvider;
		private readonly IModelProvider _ModelProvider;
		private readonly IMapObjectsCollection _MapObjectsCollection;

		public IEnumerable<MapFeatureObservableProperty> GetMapFeaturesObservableProperties() =>
			_ModelProvider
				.TypesToInitializedModules
				.SelectMany(typeToModule => _ObservablePropertiesMetadataProvider
					.TypesToObservablePropertiesManagers[typeToModule.Key.SystemType]
					.GetAllObservableProperties(typeToModule.Value)
					.Select(observableProperty => new MapFeatureObservableProperty
					{
						TypeFullName = typeToModule.Key.FullName,
						Id = typeToModule.Key.FullName,
						ObservableProperty = observableProperty
					}))
				.Concat(
					_MapObjectsCollection
						.GetAll<Geometry>()
						.SelectMany(geometry => _ObservablePropertiesMetadataProvider
							.TypesToObservablePropertiesManagers[geometry.GetType()]
							.GetAllObservableProperties(geometry)
							.Select(observableProperty => new MapFeatureObservableProperty
							{
								TypeFullName = geometry.GetType().FullName,
								Id = ((IActor)geometry).Id.ToString(),
								ObservableProperty = observableProperty
							})
						)
				);

		public IObservable<MapFeatureObservableProperty> MapFeaturesObservablePropertiesObservable { get; }

		public void SetMapFeatureObservableProperty(MapFeatureObservableProperty mapFeatureObservableProperty)
		{
			if (mapFeatureObservableProperty.TypeFullName == mapFeatureObservableProperty.Id)
			{
				// Handle as module.
				var (moduleType, module) =
					_ModelProvider.TypesToInitializedModules.First(typeToInitializedModule =>
						typeToInitializedModule.Key.FullName == mapFeatureObservableProperty.TypeFullName);

				_ObservablePropertiesMetadataProvider
					.TypesToObservablePropertiesManagers[moduleType.SystemType]
					.SetObservableProperty(module, mapFeatureObservableProperty.ObservableProperty);
			}
			else
			{
				var feature = _MapObjectsCollection.GetAll<Geometry>().OfType<IActor>().First(actor =>
					actor.Id.ToString() == mapFeatureObservableProperty.Id);

				_ObservablePropertiesMetadataProvider.TypesToObservablePropertiesManagers[feature.GetType()]
					.SetObservableProperty(feature, mapFeatureObservableProperty.ObservableProperty);
			}
		}
	}
}