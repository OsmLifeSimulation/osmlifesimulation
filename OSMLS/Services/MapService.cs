using System;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using OSMLS.Map;
using OSMLS.Map.Metadata;
using OSMLS.Map.Properties;
using static OSMLS.Map.MapService;

namespace OSMLS.Services
{
	internal class MapService : MapServiceBase
	{
		public MapService(IMapFeaturesProvider mapFeaturesProvider,
			IMapFeaturesMetadataProvider mapFeaturesMetadataProvider,
			IMapFeaturesObservablePropertiesProvider mapFeaturesObservablePropertiesProvider)
		{
			_MapFeaturesProvider = mapFeaturesProvider;
			_MapFeaturesMetadataProvider = mapFeaturesMetadataProvider;
			_MapFeaturesObservablePropertiesProvider = mapFeaturesObservablePropertiesProvider;
		}

		private readonly IMapFeaturesProvider _MapFeaturesProvider;
		private readonly IMapFeaturesMetadataProvider _MapFeaturesMetadataProvider;
		private readonly IMapFeaturesObservablePropertiesProvider _MapFeaturesObservablePropertiesProvider;

		private static async Task WaitCancellation(CancellationToken cancellationToken)
		{
			var taskCompletionSource = new TaskCompletionSource<bool>();
			cancellationToken.Register(callback =>
					((TaskCompletionSource<bool>) callback).SetResult(true),
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
			_MapFeaturesMetadataProvider.MapFeaturesMetadataObservable.Subscribe(
				async onNext => await responseStream.WriteAsync(onNext),
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
			_MapFeaturesProvider.MapFeaturesObservable.Subscribe(
				async onNext => await responseStream.WriteAsync(onNext),
				context.CancellationToken);

			await WaitCancellation(context.CancellationToken);
		}

		public override async Task GetRemoveMapFeatureEventsUpdates(Empty request,
			IServerStreamWriter<RemoveMapFeatureEvent> responseStream, ServerCallContext context)
		{
			_MapFeaturesProvider.RemoveMapFeatureEventsObservable.Subscribe(
				async onNext => await responseStream.WriteAsync(onNext),
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
			_MapFeaturesObservablePropertiesProvider.MapFeaturesObservablePropertiesObservable.Subscribe(
				async onNext => await responseStream.WriteAsync(onNext),
				context.CancellationToken);

			await WaitCancellation(context.CancellationToken);
		}

		public override Task<Empty> SetMapFeatureObservableProperty(MapFeatureObservableProperty request, ServerCallContext context)
		{
			_MapFeaturesObservablePropertiesProvider.SetMapFeatureObservableProperty(request);

			return Task.FromResult(new Empty());
		}
	}
}