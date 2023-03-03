using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Moq;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using NUnit.Framework;
using OSMLS.Features;
using OSMLS.Map;
using OSMLS.Model.Objects;
using OSMLS.Tests.Mocks;
using OSMLSGlobalLibrary.Observable.Geometries.Actor;

namespace OSMLS.Tests.Features
{
	public class MapFeaturesProviderTest
	{
		private static string GetExpectedGeoJson(PointActor testGeometryActor)
			=> new GeoJsonWriter().Write(new Feature(
				testGeometryActor,
				new AttributesTable { { "id", testGeometryActor.Id } }
			));

		[Test]
		public void ShouldGetMapFeaturesProperly()
		{
			var testGeometryActor = new PointActor(new Coordinate());
			var mapObjectsCollectionMock = MocksComposer.ComposeMapObjectsCollectionMock(testGeometryActor);

			var actualMapFeatures =
				new MapFeaturesProvider(mapObjectsCollectionMock.Object).GetMapFeatures().ToList();

			Assert.That(actualMapFeatures, Has.Count.EqualTo(1));
			Assert.That(actualMapFeatures, Has.One.Matches<MapFeature>(
				mapFeature =>
					mapFeature.TypeFullName == testGeometryActor.GetType().FullName &&
					mapFeature.GeoJson == GetExpectedGeoJson(testGeometryActor)
			));
		}

		[Test]
		public void ShouldProvideMapFeaturesObservableProperly()
		{
			var testGeometryActor = new PointActor(new Coordinate());

			var mapObjectsCollectionMock = MocksComposer.ComposeEmptyMapObjectsCollectionMock();

			var actualMapFeatures = new List<MapFeature>();

			var mapFeaturesObservableSubscription =
				new MapFeaturesProvider(mapObjectsCollectionMock.Object)
					.MapFeaturesObservable
					.Subscribe(feature => actualMapFeatures.Add(feature));

			using (mapFeaturesObservableSubscription)
				mapObjectsCollectionMock.Raise(
					mapObjectsCollection => mapObjectsCollection.CollectionChanged += null,
					new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, testGeometryActor)
				);

			Assert.That(actualMapFeatures, Has.Count.EqualTo(1));
			Assert.That(actualMapFeatures, Has.One.Matches<MapFeature>(
				mapFeature =>
					mapFeature.TypeFullName == testGeometryActor.GetType().FullName &&
					mapFeature.GeoJson == GetExpectedGeoJson(testGeometryActor)
			));
		}

		[Test]
		public void ShouldProvideMapFeaturesObservableWithCoordinatesChangedSubscriptionProperly()
		{
			var testGeometryActor = new PointActor(new Coordinate());

			var mapObjectsCollectionMock = MocksComposer.ComposeEmptyMapObjectsCollectionMock();

			var actualMapFeatures = new List<MapFeature>();

			var mapFeaturesObservableSubscription =
				new MapFeaturesProvider(mapObjectsCollectionMock.Object)
					.MapFeaturesObservable
					.Subscribe(feature => actualMapFeatures.Add(feature));

			using (mapFeaturesObservableSubscription)
			{
				mapObjectsCollectionMock.Raise(
					mapObjectsCollection => mapObjectsCollection.CollectionChanged += null,
					new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, testGeometryActor)
				);

				testGeometryActor.X += 1;

				Assert.That(actualMapFeatures, Has.Count.EqualTo(2));
				Assert.That(actualMapFeatures, Has.One.Matches<MapFeature>(
					mapFeature =>
						mapFeature.TypeFullName == testGeometryActor.GetType().FullName &&
						mapFeature.GeoJson == GetExpectedGeoJson(testGeometryActor)
				));
			}
		}

		[Test]
		public void ShouldProvideMapFeaturesObservableWithCoordinatesChangedUnsubscriptionProperly()
		{
			var testGeometryActor = new PointActor(new Coordinate());

			var mapObjectsCollectionMock = MocksComposer.ComposeEmptyMapObjectsCollectionMock();

			var actualMapFeatures = new List<MapFeature>();

			var mapFeaturesObservableSubscription =
				new MapFeaturesProvider(mapObjectsCollectionMock.Object)
					.MapFeaturesObservable
					.Subscribe(feature => actualMapFeatures.Add(feature));

			using (mapFeaturesObservableSubscription)
			{
				mapObjectsCollectionMock.Raise(
					mapObjectsCollection => mapObjectsCollection.CollectionChanged += null,
					new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, testGeometryActor)
				);

				mapObjectsCollectionMock.Raise(
					mapObjectsCollection => mapObjectsCollection.CollectionChanged += null,
					new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, testGeometryActor)
				);

				testGeometryActor.X += 1;

				Assert.That(actualMapFeatures, Has.Count.EqualTo(1));
			}
		}

		[Test]
		public void ShouldThrowArgumentOutOfRangeExceptionOnMapObjectsCollectionChangedWrongAction()
		{
			var mapObjectsCollectionMock = new Mock<IMapObjectsCollection>();

			var _ = new MapFeaturesProvider(mapObjectsCollectionMock.Object);

			Assert.Throws<ArgumentOutOfRangeException>(() =>
				mapObjectsCollectionMock.Raise(
					mapObjectsCollection => mapObjectsCollection.CollectionChanged += null,
					new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset)
				)
			);
		}

		[Test]
		public void ShouldProvideRemoveMapFeatureEventsObservableProperly()
		{
			var testGeometryActor = new PointActor(new Coordinate());

			var mapObjectsCollectionMock = MocksComposer.ComposeEmptyMapObjectsCollectionMock();

			var actualRemoveMapFeatureEvents = new List<RemoveMapFeatureEvent>();

			var mapFeaturesObservableSubscription =
				new MapFeaturesProvider(mapObjectsCollectionMock.Object)
					.RemoveMapFeatureEventsObservable
					.Subscribe(removeMapFeatureEvent => actualRemoveMapFeatureEvents.Add(removeMapFeatureEvent));

			using (mapFeaturesObservableSubscription)
			{
				mapObjectsCollectionMock.Raise(
					mapObjectsCollection => mapObjectsCollection.CollectionChanged += null,
					new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, testGeometryActor)
				);

				Assert.That(actualRemoveMapFeatureEvents, Has.Count.EqualTo(1));
				Assert.That(actualRemoveMapFeatureEvents, Has.One.Matches<RemoveMapFeatureEvent>(
					mapFeature =>
						mapFeature.TypeFullName == testGeometryActor.GetType().FullName &&
						mapFeature.Id == testGeometryActor.Id.ToString()
				));
			}
		}
	}
}