using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Moq;
using MoreLinq.Extensions;
using NUnit.Framework;
using OSMLS.Features;
using OSMLS.Features.Metadata;
using OSMLS.Features.Properties;
using OSMLS.Map;
using MapService = OSMLS.Services.MapService;

namespace OSMLS.Tests.Services
{
	public class MapServiceTest
	{
		private static ICollection<T> ComposeTestDataCollection<T>() where T : new() => Enumerable.Range(0, 5)
			.Select(_ => new T())
			.ToImmutableList();

		private static Mock<IServerStreamWriter<T>> ComposeResponseStreamMock<T>()
		{
			var responseStreamMock = new Mock<IServerStreamWriter<T>>();

			responseStreamMock
				.Setup(responseStream => responseStream.WriteAsync(It.IsAny<T>()))
				.Verifiable();

			return responseStreamMock;
		}

		private static void VerifyWrites<T>(Mock<IServerStreamWriter<T>> responseStreamMock, ICollection<T> data)
		{
			responseStreamMock.Verify(
				responseStream => responseStream.WriteAsync(It.IsAny<T>()),
				Times.Exactly(data.Count)
			);

			data.ForEach(item =>
				responseStreamMock.Verify(responseStream => responseStream.WriteAsync(item))
			);
		}

		private class TestServerCallContext : ServerCallContext
		{
			public TestServerCallContext(CancellationToken cancellationToken)
			{
				CancellationTokenCore = cancellationToken;
			}

			protected override Task WriteResponseHeadersAsyncCore(Metadata responseHeaders)
			{
				throw new NotImplementedException();
			}

			protected override ContextPropagationToken CreatePropagationTokenCore(ContextPropagationOptions options)
			{
				throw new NotImplementedException();
			}

			protected override string MethodCore => throw new NotImplementedException();

			protected override string HostCore => throw new NotImplementedException();

			protected override string PeerCore => throw new NotImplementedException();

			protected override DateTime DeadlineCore => throw new NotImplementedException();

			protected override Metadata RequestHeadersCore => throw new NotImplementedException();

			protected override CancellationToken CancellationTokenCore { get; }

			protected override Metadata ResponseTrailersCore => throw new NotImplementedException();

			protected override Status StatusCore { get; set; }

			protected override WriteOptions WriteOptionsCore { get; set; }

			protected override AuthContext AuthContextCore => throw new NotImplementedException();
		}

		private static IObservable<T> ComposeTestDataObservableWithCachedData<T>(IEnumerable<T> testData)
		{
			var testDataSubject = new ReplaySubject<T>();
			testData.ForEach(item => testDataSubject.OnNext(item));

			return testDataSubject;
		}

		[Test]
		public async Task ShouldGetMapFeaturesMetadataProperly()
		{
			var expectedResult = ComposeTestDataCollection<MapFeaturesMetadata>();

			var mapFeaturesMetadataProviderMock = new Mock<IMapFeaturesMetadataProvider>();
			mapFeaturesMetadataProviderMock
				.Setup(mapFeaturesMetadataProvider => mapFeaturesMetadataProvider.GetMapFeaturesMetadata())
				.Returns(expectedResult)
				.Verifiable();

			var mapService = new MapService(
				mapFeaturesMetadataProviderMock.Object,
				new Mock<IMapFeaturesProvider>().Object,
				new Mock<IMapFeaturesObservablePropertiesProvider>().Object
			);

			var responseStreamMock = ComposeResponseStreamMock<MapFeaturesMetadata>();

			await mapService.GetMapFeaturesMetadata(
				new Empty(),
				responseStreamMock.Object,
				new Mock<ServerCallContext>().Object
			);

			VerifyWrites(responseStreamMock, expectedResult);
		}

		[Test]
		public async Task ShouldGetMapFeaturesMetadataUpdatesProperly()
		{
			var expectedResult = ComposeTestDataCollection<MapFeaturesMetadata>();

			var mapFeaturesMetadataProviderMock = new Mock<IMapFeaturesMetadataProvider>();
			mapFeaturesMetadataProviderMock
				.Setup(mapFeaturesMetadataProvider => mapFeaturesMetadataProvider.MapFeaturesMetadataObservable)
				.Returns(ComposeTestDataObservableWithCachedData(expectedResult))
				.Verifiable();

			var mapService = new MapService(
				mapFeaturesMetadataProviderMock.Object,
				new Mock<IMapFeaturesProvider>().Object,
				new Mock<IMapFeaturesObservablePropertiesProvider>().Object
			);

			var responseStreamMock = ComposeResponseStreamMock<MapFeaturesMetadata>();

			var cancellationTokenSource = new CancellationTokenSource();
			var serviceTask = mapService.GetMapFeaturesMetadataUpdates(
				new Empty(),
				responseStreamMock.Object,
				new TestServerCallContext(cancellationTokenSource.Token)
			);

			cancellationTokenSource.Cancel();

			await serviceTask;

			VerifyWrites(responseStreamMock, expectedResult);
		}

		[Test]
		public async Task ShouldGetMapFeaturesProperly()
		{
			var expectedResult = ComposeTestDataCollection<MapFeature>();

			var mapFeaturesProviderMock = new Mock<IMapFeaturesProvider>();
			mapFeaturesProviderMock
				.Setup(mapFeaturesProvider => mapFeaturesProvider.GetMapFeatures())
				.Returns(expectedResult)
				.Verifiable();

			var mapService = new MapService(
				new Mock<IMapFeaturesMetadataProvider>().Object,
				mapFeaturesProviderMock.Object,
				new Mock<IMapFeaturesObservablePropertiesProvider>().Object
			);

			var responseStreamMock = ComposeResponseStreamMock<MapFeature>();

			await mapService.GetMapFeatures(
				new Empty(),
				responseStreamMock.Object,
				new Mock<ServerCallContext>().Object
			);

			VerifyWrites(responseStreamMock, expectedResult);
		}

		[Test]
		public async Task ShouldGetMapFeaturesUpdatesProperly()
		{
			var expectedResult = ComposeTestDataCollection<MapFeature>();

			var mapFeaturesProviderMock = new Mock<IMapFeaturesProvider>();
			mapFeaturesProviderMock
				.Setup(mapFeaturesProvider => mapFeaturesProvider.MapFeaturesObservable)
				.Returns(ComposeTestDataObservableWithCachedData(expectedResult))
				.Verifiable();

			var mapService = new MapService(
				new Mock<IMapFeaturesMetadataProvider>().Object,
				mapFeaturesProviderMock.Object,
				new Mock<IMapFeaturesObservablePropertiesProvider>().Object
			);

			var responseStreamMock = ComposeResponseStreamMock<MapFeature>();

			var cancellationTokenSource = new CancellationTokenSource();
			var serviceTask = mapService.GetMapFeaturesUpdates(
				new Empty(),
				responseStreamMock.Object,
				new TestServerCallContext(cancellationTokenSource.Token)
			);

			cancellationTokenSource.Cancel();

			await serviceTask;

			VerifyWrites(responseStreamMock, expectedResult);
		}

		[Test]
		public async Task ShouldGetRemoveMapFeatureEventsUpdatesProperly()
		{
			var expectedResult = ComposeTestDataCollection<RemoveMapFeatureEvent>();

			var mapFeaturesProviderMock = new Mock<IMapFeaturesProvider>();
			mapFeaturesProviderMock
				.Setup(mapFeaturesProvider => mapFeaturesProvider.RemoveMapFeatureEventsObservable)
				.Returns(ComposeTestDataObservableWithCachedData(expectedResult))
				.Verifiable();

			var mapService = new MapService(
				new Mock<IMapFeaturesMetadataProvider>().Object,
				mapFeaturesProviderMock.Object,
				new Mock<IMapFeaturesObservablePropertiesProvider>().Object
			);

			var responseStreamMock = ComposeResponseStreamMock<RemoveMapFeatureEvent>();

			var cancellationTokenSource = new CancellationTokenSource();
			var serviceTask = mapService.GetRemoveMapFeatureEventsUpdates(
				new Empty(),
				responseStreamMock.Object,
				new TestServerCallContext(cancellationTokenSource.Token)
			);

			cancellationTokenSource.Cancel();

			await serviceTask;

			VerifyWrites(responseStreamMock, expectedResult);
		}

		[Test]
		public async Task ShouldGetMapFeaturesObservablePropertiesProperly()
		{
			var expectedResult = ComposeTestDataCollection<MapFeatureObservableProperty>();

			var mapFeaturesObservablePropertiesProviderMock = new Mock<IMapFeaturesObservablePropertiesProvider>();
			mapFeaturesObservablePropertiesProviderMock
				.Setup(mapFeaturesObservablePropertiesProvider =>
					mapFeaturesObservablePropertiesProvider.GetMapFeaturesObservableProperties()
				)
				.Returns(expectedResult)
				.Verifiable();

			var mapService = new MapService(
				new Mock<IMapFeaturesMetadataProvider>().Object,
				new Mock<IMapFeaturesProvider>().Object,
				mapFeaturesObservablePropertiesProviderMock.Object
			);

			var responseStreamMock = ComposeResponseStreamMock<MapFeatureObservableProperty>();

			await mapService.GetMapFeaturesObservableProperties(
				new Empty(),
				responseStreamMock.Object,
				new Mock<ServerCallContext>().Object
			);

			VerifyWrites(responseStreamMock, expectedResult);
		}

		[Test]
		public async Task ShouldGetMapFeaturesObservablePropertiesUpdatesProperly()
		{
			var expectedResult = ComposeTestDataCollection<MapFeatureObservableProperty>();

			var mapFeaturesObservablePropertiesProviderMock = new Mock<IMapFeaturesObservablePropertiesProvider>();
			mapFeaturesObservablePropertiesProviderMock
				.Setup(mapFeaturesObservablePropertiesProvider =>
					mapFeaturesObservablePropertiesProvider.MapFeaturesObservablePropertiesObservable
				)
				.Returns(ComposeTestDataObservableWithCachedData(expectedResult))
				.Verifiable();

			var mapService = new MapService(
				new Mock<IMapFeaturesMetadataProvider>().Object,
				new Mock<IMapFeaturesProvider>().Object,
				mapFeaturesObservablePropertiesProviderMock.Object
			);

			var responseStreamMock = ComposeResponseStreamMock<MapFeatureObservableProperty>();

			var cancellationTokenSource = new CancellationTokenSource();
			var serviceTask = mapService.GetMapFeaturesObservablePropertiesUpdates(
				new Empty(),
				responseStreamMock.Object,
				new TestServerCallContext(cancellationTokenSource.Token)
			);

			cancellationTokenSource.Cancel();

			await serviceTask;

			VerifyWrites(responseStreamMock, expectedResult);
		}

		[Test]
		public async Task ShouldSetMapFeatureObservablePropertyProperly()
		{
			var testRequest = new MapFeatureObservableProperty();

			var mapFeaturesObservablePropertiesProviderMock = new Mock<IMapFeaturesObservablePropertiesProvider>();
			mapFeaturesObservablePropertiesProviderMock
				.Setup(mapFeaturesObservablePropertiesProvider =>
					mapFeaturesObservablePropertiesProvider.SetMapFeatureObservableProperty(testRequest)
				)
				.Verifiable();

			var mapService = new MapService(
				new Mock<IMapFeaturesMetadataProvider>().Object,
				new Mock<IMapFeaturesProvider>().Object,
				mapFeaturesObservablePropertiesProviderMock.Object
			);

			await mapService.SetMapFeatureObservableProperty(
				testRequest,
				new Mock<ServerCallContext>().Object
			);

			mapFeaturesObservablePropertiesProviderMock.Verify(
				mapFeaturesObservablePropertiesProvider =>
					mapFeaturesObservablePropertiesProvider.SetMapFeatureObservableProperty(testRequest)
			);
		}
	}
}