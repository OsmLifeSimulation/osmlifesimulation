using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Subjects;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using OSMLS.Map;
using OSMLS.Model.Objects;
using OSMLSGlobalLibrary.Observable.Geometries.Actor;

namespace OSMLS.Features
{
	public class MapFeaturesProvider : IMapFeaturesProvider
	{
		public MapFeaturesProvider(IMapObjectsCollection mapObjectsCollection)
		{
			_MapObjectsCollection = mapObjectsCollection;
			_GeoJsonWriter = new GeoJsonWriter();

			var mapFeaturesSubject = new Subject<MapFeature>();
			MapFeaturesObservable = mapFeaturesSubject;

			var removeMapFeatureEventsSubject = new Subject<RemoveMapFeatureEvent>();
			RemoveMapFeatureEventsObservable = removeMapFeatureEventsSubject;

			mapObjectsCollection.CollectionChanged += (_, args) =>
			{
				void CoordinatesChangedEventHandler(object obj, EventArgs eventArgs) =>
					mapFeaturesSubject.OnNext(GetMapFeature(_GeoJsonWriter, obj));

				// ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
				switch (args.Action)
				{
					case NotifyCollectionChangedAction.Add:
						foreach (var actor in args.NewItems!.OfType<IActor>())
						{
							actor.CoordinatesChanged += CoordinatesChangedEventHandler;

							mapFeaturesSubject.OnNext(GetMapFeature(_GeoJsonWriter, actor));
						}

						break;
					case NotifyCollectionChangedAction.Remove:
						foreach (var actor in args.OldItems!.OfType<IActor>())
						{
							actor.CoordinatesChanged -= CoordinatesChangedEventHandler;

							removeMapFeatureEventsSubject.OnNext(new RemoveMapFeatureEvent
							{
								TypeFullName = actor.GetType().FullName,
								Id = actor.Id.ToString()
							});
						}

						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			};
		}

		private readonly GeoJsonWriter _GeoJsonWriter;

		private readonly IMapObjectsCollection _MapObjectsCollection;

		public IEnumerable<MapFeature> GetMapFeatures() =>
			_MapObjectsCollection.GetAll<Geometry>()
				.OfType<IActor>()
				.Select(geometry => GetMapFeature(_GeoJsonWriter, geometry));

		public IObservable<MapFeature> MapFeaturesObservable { get; }

		public IObservable<RemoveMapFeatureEvent> RemoveMapFeatureEventsObservable { get; }

		private static MapFeature GetMapFeature(GeoJsonWriter geoJsonWriter, object geometryActor)
		{
			var geometry = geometryActor as Geometry;
			var actor = (geometryActor as IActor)!;

			return new MapFeature
			{
				TypeFullName = actor.GetType().FullName,
				GeoJson = geoJsonWriter.Write(new Feature(geometry, new AttributesTable { { "id", actor.Id } }))
			};
		}
	}
}