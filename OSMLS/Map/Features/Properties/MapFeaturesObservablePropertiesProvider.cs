using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Subjects;
using NetTopologySuite.Geometries;
using OSMLS.Map.Features.Metadata;
using OSMLS.Model;
using OSMLSGlobalLibrary.Geometries.Observable.Actor;

namespace OSMLS.Map.Features.Properties
{
	public class MapFeaturesObservablePropertiesProvider : IMapFeaturesObservablePropertiesProvider
	{
		public MapFeaturesObservablePropertiesProvider(
			IObservablePropertiesMetadataProvider observablePropertiesMetadataProvider,
			IMapObjectsCollection mapObjectsCollection)
		{
			_ObservablePropertiesMetadataProvider = observablePropertiesMetadataProvider;
			_MapObjectsCollection = mapObjectsCollection;
			var mapFeaturesObservablePropertiesSubject = new Subject<MapFeatureObservableProperty>();
			MapFeaturesObservablePropertiesObservable = mapFeaturesObservablePropertiesSubject;

			mapObjectsCollection.CollectionChanged += (_, args) =>
			{
				void PropertyChangedEventHandler(object obj, PropertyChangedEventArgs eventArgs)
				{
					var type = obj.GetType();
					mapFeaturesObservablePropertiesSubject.OnNext(
						new MapFeatureObservableProperty
						{
							TypeFullName = type.FullName,
							Id = ((IActor) obj).Id.ToString(),
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
		private readonly IMapObjectsCollection _MapObjectsCollection;

		public IEnumerable<MapFeatureObservableProperty> GetMapFeaturesObservableProperties() => _MapObjectsCollection
			.GetAll<Geometry>()
			.SelectMany(geometry => _ObservablePropertiesMetadataProvider
				.TypesToObservablePropertiesManagers[geometry.GetType()]
				.GetAllObservableProperties(geometry)
				.Select(observableProperty => new MapFeatureObservableProperty
				{
					TypeFullName = geometry.GetType().FullName,
					Id = ((IActor) geometry).Id.ToString(),
					ObservableProperty = observableProperty
				})
			);

		public IObservable<MapFeatureObservableProperty> MapFeaturesObservablePropertiesObservable { get; }

		public void SetMapFeatureObservableProperty(MapFeatureObservableProperty mapFeatureObservableProperty)
		{
			var feature = _MapObjectsCollection.GetAll<Geometry>().First(geometry =>
				((IActor) geometry).Id.ToString() == mapFeatureObservableProperty.Id);

			_ObservablePropertiesMetadataProvider.TypesToObservablePropertiesManagers[feature.GetType()]
				.SetObservableProperty(feature, mapFeatureObservableProperty.ObservableProperty);
		}
	}
}