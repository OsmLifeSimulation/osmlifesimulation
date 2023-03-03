using System;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using OSMLS.Features;
using OSMLS.Features.Metadata;
using OSMLS.Features.Properties;
using OSMLS.Map;
using static OSMLS.Map.MapService;

namespace OSMLS.Services
{
	public class MapService : MapServiceBase
	{
		public MapService(
			IMapFeaturesMetadataProvider mapFeaturesMetadataProvider,
			IMapFeaturesProvider mapFeaturesProvider,
			IMapFeaturesObservablePropertiesProvider mapFeaturesObservablePropertiesProvider)
		{
			_MapFeaturesMetadataProvider = mapFeaturesMetadataProvider;
			_MapFeaturesProvider = mapFeaturesProvider;
			_MapFeaturesObservablePropertiesProvider = mapFeaturesObservablePropertiesProvider;
		}

		private readonly IMapFeaturesMetadataProvider _MapFeaturesMetadataProvider;
		private readonly IMapFeaturesProvider _MapFeaturesProvider;
		private readonly IMapFeaturesObservablePropertiesProvider _MapFeaturesObservablePropertiesProvider;

		private static async Task WaitCancellation(CancellationToken cancellationToken)
		{
			var taskCompletionSource = new TaskCompletionSource<bool>();
			cancellationToken.Register(callback =>
					((TaskCompletionSource<bool>)callback).SetResult(true),
				taskCompletionSource
			);

			await taskCompletionSource.Task;
		}

		public override async Task GetMapFeaturesMetadata(Empty request,
			IServerStreamWriter<MapFeaturesMetadata> responseStream, ServerCallContext context)
		{
			foreach (var mapFeaturesMetadata in _MapFeaturesMetadataProvider.GetMapFeaturesMetadata())
				await responseStream.WriteAsync(mapFeaturesMetadata);
		}

		public override async Task GetMapFeaturesMetadataUpdates(Empty request,
			IServerStreamWriter<MapFeaturesMetadata> responseStream, ServerCallContext context)
		{
			var writeLock = new object();
			_MapFeaturesMetadataProvider.MapFeaturesMetadataObservable.Subscribe(
				onNext =>
				{
					lock (writeLock)
						responseStream.WriteAsync(onNext);
				},
				context.CancellationToken);

			await WaitCancellation(context.CancellationToken);
		}

		public override async Task GetMapFeatures(Empty request, IServerStreamWriter<MapFeature> responseStream,
			ServerCallContext context)
		{
			foreach (var mapFeature in _MapFeaturesProvider.GetMapFeatures())
				await responseStream.WriteAsync(mapFeature);
		}

		public override async Task GetMapFeaturesUpdates(Empty request, IServerStreamWriter<MapFeature> responseStream,
			ServerCallContext context)
		{
			var writeLock = new object();
			_MapFeaturesProvider.MapFeaturesObservable.Subscribe(
				onNext =>
				{
					lock (writeLock)
						responseStream.WriteAsync(onNext);
				},
				context.CancellationToken);

			await WaitCancellation(context.CancellationToken);
		}

		public override async Task GetRemoveMapFeatureEventsUpdates(Empty request,
			IServerStreamWriter<RemoveMapFeatureEvent> responseStream, ServerCallContext context)
		{
			var writeLock = new object();
			_MapFeaturesProvider.RemoveMapFeatureEventsObservable.Subscribe(
				onNext =>
				{
					lock (writeLock)
						responseStream.WriteAsync(onNext);
				},
				context.CancellationToken);

			await WaitCancellation(context.CancellationToken);
		}

		public override async Task GetMapFeaturesObservableProperties(Empty request,
			IServerStreamWriter<MapFeatureObservableProperty> responseStream, ServerCallContext context)
		{
			foreach (var mapFeatureObservableProperty in
			         _MapFeaturesObservablePropertiesProvider.GetMapFeaturesObservableProperties())
				await responseStream.WriteAsync(mapFeatureObservableProperty);
		}

		public override async Task GetMapFeaturesObservablePropertiesUpdates(Empty request,
			IServerStreamWriter<MapFeatureObservableProperty> responseStream,
			ServerCallContext context)
		{
			var writeLock = new object();
			_MapFeaturesObservablePropertiesProvider.MapFeaturesObservablePropertiesObservable.Subscribe(
				onNext =>
				{
					lock (writeLock)
						responseStream.WriteAsync(onNext);
				},
				context.CancellationToken);

			await WaitCancellation(context.CancellationToken);
		}

		public override Task<Empty> SetMapFeatureObservableProperty(MapFeatureObservableProperty request,
			ServerCallContext context)
		{
			_MapFeaturesObservablePropertiesProvider.SetMapFeatureObservableProperty(request);

			return Task.FromResult(new Empty());
		}
	}
}