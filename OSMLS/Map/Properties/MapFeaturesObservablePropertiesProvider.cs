using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Subjects;
using System.Runtime.Loader;
using NetTopologySuite.Geometries;
using OSMLS.Map.Metadata;
using OSMLS.Model;
using OSMLS.Services;
using OSMLSGlobalLibrary.Observable.Geometries.Actor;

namespace OSMLS.Map.Properties
{
	public class MapFeaturesObservablePropertiesProvider : IMapFeaturesObservablePropertiesProvider
	{
		public MapFeaturesObservablePropertiesProvider(
			IObservablePropertiesMetadataProvider observablePropertiesMetadataProvider,
			IModelService modelService,
			IMapObjectsCollection mapObjectsCollection)
		{
			_ObservablePropertiesMetadataProvider = observablePropertiesMetadataProvider;
			_ModelService = modelService;
			_MapObjectsCollection = mapObjectsCollection;
			var mapFeaturesObservablePropertiesSubject = new Subject<MapFeatureObservableProperty>();
			MapFeaturesObservablePropertiesObservable = mapFeaturesObservablePropertiesSubject;

			modelService.ModuleInitialized += (_, args) =>
			{
				void PropertyChangedEventHandler(object obj, PropertyChangedEventArgs eventArgs)
				{
					var type = obj.GetType();
					mapFeaturesObservablePropertiesSubject.OnNext(
						new MapFeatureObservableProperty
						{
							TypeFullName = type.FullName,
							Id = type.FullName,
							ObservableProperty = _ObservablePropertiesMetadataProvider
								.TypesToObservablePropertiesManagers[type]
								.TryGetObservableProperty(obj, eventArgs.PropertyName)
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

			mapObjectsCollection.CollectionChanged += (_, args) =>
			{
				void PropertyChangedEventHandler(object obj, PropertyChangedEventArgs eventArgs)
				{
					var type = obj.GetType();
					mapFeaturesObservablePropertiesSubject.OnNext(
						new MapFeatureObservableProperty
						{
							TypeFullName = type.FullName,
							Id = ((IActor)obj).Id.ToString(),
							ObservableProperty = _ObservablePropertiesMetadataProvider
								.TypesToObservablePropertiesManagers[type]
								.TryGetObservableProperty(obj, eventArgs.PropertyName)
						}
					);
				}

				IActor actor;

				// ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
				switch (args.Action)
				{
					case NotifyCollectionChangedAction.Add:
						actor = args.NewItems!.Cast<IActor>().First();
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


						break;
					case NotifyCollectionChangedAction.Remove:
						actor = args.OldItems!.Cast<IActor>().First();
						actor.PropertyChanged -= PropertyChangedEventHandler;

						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			};
		}

		private readonly IObservablePropertiesMetadataProvider _ObservablePropertiesMetadataProvider;
		private readonly IModelService _ModelService;
		private readonly IMapObjectsCollection _MapObjectsCollection;

		public IEnumerable<MapFeatureObservableProperty> GetMapFeaturesObservableProperties() =>
			_ModelService
				.TypesToInitializedModules
				.SelectMany(typeToModule => _ObservablePropertiesMetadataProvider
					.TypesToObservablePropertiesManagers[typeToModule.Key]
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
					_ModelService.TypesToInitializedModules.First(typeToInitializedModule =>
						typeToInitializedModule.Key.FullName == mapFeatureObservableProperty.TypeFullName);

				_ObservablePropertiesMetadataProvider
					.TypesToObservablePropertiesManagers[moduleType]
					.SetObservableProperty(module, mapFeatureObservableProperty.ObservableProperty);
			}
			else
			{
				var feature = _MapObjectsCollection.GetAll<Geometry>().First(geometry =>
					((IActor)geometry).Id.ToString() == mapFeatureObservableProperty.Id);

				_ObservablePropertiesMetadataProvider.TypesToObservablePropertiesManagers[feature.GetType()]
					.SetObservableProperty(feature, mapFeatureObservableProperty.ObservableProperty);
			}
		}
	}
}