using System;
using System.Collections.Generic;
using System.Reflection;
using Moq;
using NetTopologySuite.Geometries;
using OSMLS.Model.Objects;
using OSMLS.Types;
using OSMLS.Types.Model;

namespace OSMLS.Tests.Mocks
{
	public static class MocksComposer
	{
		private static readonly Random Random = new();

		public static Mock<IInjectedTypesProvider> ComposeTypesProviderMock(IEnumerable<IInjectedType> types)
		{
			var typesProviderMock = new Mock<IInjectedTypesProvider>();
			typesProviderMock
				.Setup(typesProvider => typesProvider.GetTypes())
				.Returns(types);

			return typesProviderMock;
		}

		public static Mock<IInjectedTypesProvider> ComposeTypesProviderMock(IInjectedType type) =>
			ComposeTypesProviderMock(new[] { type });

		public static Mock<T> ComposeInjectedTypeMock<T>() where T : class, IInjectedType
		{
			var typeMock = new Mock<T>();
			typeMock
				.Setup(type => type.FullName)
				.Returns(Guid.NewGuid().ToString());
			typeMock
				.Setup(type => type.SystemType)
				.Returns(ComposeSystemTypeMock().Object);

			return typeMock;
		}

		private static Mock<Type> ComposeSystemTypeMock()
		{
			var typeMock = new Mock<Type>();
			typeMock
				.Setup(type => type.Equals(It.IsAny<MemberInfo>()))
				.Returns(true);
			typeMock
				.Setup(type => type.GetHashCode())
				.Returns(Random.Next());

			return typeMock;
		}

		public static Mock<IMapObjectsCollection> ComposeEmptyMapObjectsCollectionMock()
		{
			var mapObjectsCollectionMock = new Mock<IMapObjectsCollection>();
			mapObjectsCollectionMock
				.Setup(mapObjectsCollection => mapObjectsCollection.GetAll<Geometry>())
				.Returns(new List<Geometry>());

			return mapObjectsCollectionMock;
		}

		public static Mock<IMapObjectsCollection> ComposeMapObjectsCollectionMock(Geometry geometry)
		{
			var mapObjectsCollectionMock = new Mock<IMapObjectsCollection>();
			mapObjectsCollectionMock
				.Setup(mapObjectsCollection => mapObjectsCollection.GetAll<Geometry>())
				.Returns(new List<Geometry> { geometry });

			return mapObjectsCollectionMock;
		}
	}
}