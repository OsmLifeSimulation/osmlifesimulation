using System;
using System.Collections.Immutable;
using System.Linq;
using NetTopologySuite.Geometries;
using NUnit.Framework;
using OSMLS.Model;

namespace OSMLS.Tests.Model
{
	public class MapObjectsCollectionTest
	{
		private static readonly Random Random = new();

		private static Coordinate ComposeTestCoordinate() => new(Random.Next(), Random.Next());

		private static ImmutableList<Point> ComposeTestPoints() => Enumerable.Range(0, Random.Next(2, 100))
			.Select(_ => new Point(ComposeTestCoordinate()))
			.ToImmutableList();

		private static ImmutableList<LineString> ComposeTestLineStrings() => Enumerable.Range(0, Random.Next(2, 100))
			.Select(_ => new LineString(Enumerable.Range(2, 100).Select(_ => ComposeTestCoordinate()).ToArray()))
			.ToImmutableList();

		[Test]
		public void ShouldManageCollectionContentProperly()
		{
			var mapObjectsCollection = new MapObjectsCollection();

			var testPoints = ComposeTestPoints();
			var testLineStrings = ComposeTestLineStrings();
			var testGeometries = testPoints.Cast<Geometry>()
				.Concat(testLineStrings)
				.ToList();

			testGeometries.ForEach(geometry => mapObjectsCollection.Add(geometry));

			CollectionAssert.AreEqual(
				testPoints,
				mapObjectsCollection.Get<Point>()
			);

			CollectionAssert.AreEqual(
				testLineStrings,
				mapObjectsCollection.Get<LineString>()
			);

			CollectionAssert.AreEqual(
				Enumerable.Empty<Geometry>(),
				mapObjectsCollection.Get<Geometry>()
			);

			CollectionAssert.AreEqual(
				testGeometries,
				mapObjectsCollection.GetAll<Geometry>()
			);

			var pointToRemove = testPoints.First();
			var linearStringToRemove = testPoints.First();
			mapObjectsCollection.Remove(pointToRemove);
			mapObjectsCollection.Remove(linearStringToRemove);

			CollectionAssert.AreEqual(
				testGeometries.Except(new[] {pointToRemove, linearStringToRemove}),
				mapObjectsCollection.GetAll<Geometry>()
			);
		}

		[Test]
		public void ShouldGetTypeItemsProperly()
		{
			var mapObjectsCollection = new MapObjectsCollection();

			var testPoints = ComposeTestPoints();
			var testLineStrings = ComposeTestLineStrings();
			var testGeometries = testPoints.Cast<Geometry>()
				.Concat(testLineStrings)
				.ToList();

			testGeometries.ForEach(geometry => mapObjectsCollection.Add(geometry));

			var typesToItems = mapObjectsCollection.GetTypeItems();

			Assert.AreEqual(3, typesToItems.Count);
			CollectionAssert.AreEqual(testPoints, typesToItems[typeof(Point)]);
			CollectionAssert.AreEqual(testLineStrings, typesToItems[typeof(LineString)]);
			CollectionAssert.AreEqual(Enumerable.Empty<Geometry>(), typesToItems[typeof(Geometry)]);
		}

		[Test]
		public void ShouldNotThrowExceptionOnNonExistentItemRemove()
		{
			var mapObjectsCollection = new MapObjectsCollection();

			Assert.DoesNotThrow(() =>
				mapObjectsCollection.Remove(new Point(new Coordinate()))
			);
		}
	}
}