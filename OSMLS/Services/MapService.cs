using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using NetTopologySuite.Features;
using NetTopologySuite.IO;
using OSMLS.Map;
using OSMLS.Model;
using OSMLSGlobalLibrary.Map;
using static OSMLS.Map.MapService;

namespace OSMLS.Services
{
	internal class MapService : MapServiceBase
	{
		public MapService(IMapObjectsCollection mapObjectsCollection)
		{
			var geoJsonWriter = new GeoJsonWriter();

			_MapFeaturesClustersObservable =
				Observable.Timer(TimeSpan.FromMilliseconds(-1), TimeSpan.FromSeconds(1))
					.Select(
						_ =>
						{
							var mapFeaturesCluster = new MapFeaturesCluster();

							foreach (var (type, mapObjects) in mapObjectsCollection.GetTypeItems())
							{
								var openLayersStyle = ((CustomStyleAttribute) type
									.GetCustomAttributes(typeof(CustomStyleAttribute), false)
									.FirstOrDefault() ?? new CustomStyleAttribute()).Style;

								if (openLayersStyle == null)
									continue;

								mapFeaturesCluster.Features.Add(new MapFeature
								{
									TypeFullName = type.FullName,
									FeaturesGeoJson = "{\"type\":\"FeatureCollection\", \"features\":" +
									                  geoJsonWriter.Write(
										                  new FeatureCollection().Concat(
											                  mapObjects
												                  .Select(
													                  geometry => new Feature(geometry,
														                  new AttributesTable())
												                  ).ToList()
										                  )
									                  ) +
									                  "}",
									OpenLayersStyle = openLayersStyle
								});
							}

							return mapFeaturesCluster;
						});
		}

		private readonly IObservable<MapFeaturesCluster> _MapFeaturesClustersObservable;

		public override async Task Updates(Empty request, IServerStreamWriter<MapFeaturesCluster> responseStream,
			ServerCallContext context)
		{
			_MapFeaturesClustersObservable.Subscribe(onNext =>
					responseStream.WriteAsync(onNext),
				context.CancellationToken
			);

			var taskCompletionSource = new TaskCompletionSource<bool>();
			context.CancellationToken.Register(callback =>
					((TaskCompletionSource<bool>) callback).SetResult(true),
				taskCompletionSource
			);

			await taskCompletionSource.Task;
		}
	}
}