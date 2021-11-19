using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Subjects;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using OSMLS.Model;
using OSMLSGlobalLibrary.Observable.Geometries.Actor;

namespace OSMLS.Map
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
				void CoordinatesChangedEventHandler(object obj, EventArgs eventArgs)
				{
					mapFeaturesSubject.OnNext(GetMapFeature(_GeoJsonWriter, obj));
				}

				IActor actor;

				// ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
				switch (args.Action)
				{
					case NotifyCollectionChangedAction.Add:
						actor = args.NewItems!.Cast<IActor>().First();

						actor.CoordinatesChanged += CoordinatesChangedEventHandler;

						mapFeaturesSubject.OnNext(GetMapFeature(_GeoJsonWriter, actor));

						break;
					case NotifyCollectionChangedAction.Remove:
						actor = args.OldItems!.Cast<IActor>().First();

						actor.CoordinatesChanged -= CoordinatesChangedEventHandler;

						removeMapFeatureEventsSubject.OnNext(new RemoveMapFeatureEvent
						{
							TypeFullName = actor.GetType().FullName,
							Id = actor.Id.ToString()
						});

						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			};
		}

		private readonly GeoJsonWriter _GeoJsonWriter;

		private readonly IMapObjectsCollection _MapObjectsCollection;

		public IEnumerable<MapFeature> GetMapFeatures() =>
			_MapObjectsCollection.GetAll<Geometry>().Select(geometry => GetMapFeature(_GeoJsonWriter, geometry));

		public IObservable<MapFeature> MapFeaturesObservable { get; }

		public IObservable<RemoveMapFeatureEvent> RemoveMapFeatureEventsObservable { get; }

		private static MapFeature GetMapFeature(GeoJsonWriter geoJsonWriter, object geometryActor)
		{
			if (geometryActor is Geometry geometry and IActor actor)
				return new MapFeature
				{
					TypeFullName = actor.GetType().FullName,
					GeoJson = geoJsonWriter.Write(new Feature(geometry, new AttributesTable
							{
								{
									"id", actor.Id
								}
							}
						)
					)
				};

			throw new ArgumentException();
		}
	}
}