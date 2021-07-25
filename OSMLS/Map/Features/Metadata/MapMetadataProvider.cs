using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using NetTopologySuite.Geometries;
using OSMLS.Map.Features.Properties;
using OSMLS.Model;
using OSMLSGlobalLibrary.Geometries.Observable.Actor;
using OSMLSGlobalLibrary.Map;

namespace OSMLS.Map.Features.Metadata
{
	public class MapMetadataProvider : IMapFeaturesMetadataProvider, IObservablePropertiesMetadataProvider
	{
		public MapMetadataProvider(INotifyAssemblyAdded notifyAssemblyAdded)
		{
			TypesToObservablePropertiesManagers = AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(assembly => assembly.GetTypes())
				.ToDictionary(type => type, type => new ObservablePropertiesManager(type))
				.ToImmutableDictionary();

			var mapFeaturesMetadataSubject = new Subject<MapFeaturesMetadata>();
			MapFeaturesMetadataObservable = mapFeaturesMetadataSubject.AsObservable();
			notifyAssemblyAdded.AssemblyAdded += (_, args) =>
			{
				var assemblyTypes = args.Assembly.GetTypes();

				TypesToObservablePropertiesManagers = TypesToObservablePropertiesManagers.AddRange(
					assemblyTypes.ToDictionary(
						type => type,
						type => new ObservablePropertiesManager(type)
					)
				);

				GetMapFeaturesMetadata(assemblyTypes)
					.ToList()
					.ForEach(mapFeaturesMetadata => mapFeaturesMetadataSubject.OnNext(mapFeaturesMetadata));
			};
		}

		public IEnumerable<MapFeaturesMetadata> GetMapFeaturesMetadata() =>
			AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => GetMapFeaturesMetadata(assembly.GetTypes()));

		public IImmutableDictionary<Type, ObservablePropertiesManager> TypesToObservablePropertiesManagers { get; set; }

		public IObservable<MapFeaturesMetadata> MapFeaturesMetadataObservable { get; }

		private IEnumerable<MapFeaturesMetadata> GetMapFeaturesMetadata(IEnumerable<Type> types)
		{
			return types
				.Where(type => type.IsSubclassOf(typeof(Geometry)) &&
				               typeof(IActor).IsAssignableFrom(type))
				.Select(type =>
				{
					var mapFeaturesMetadata = new MapFeaturesMetadata
					{
						TypeFullName = type.FullName,
						OpenLayersStyle = ((CustomStyleAttribute) type
							.GetCustomAttributes(typeof(CustomStyleAttribute), false)
							.FirstOrDefault() ?? new CustomStyleAttribute()).Style,
					};

					mapFeaturesMetadata.ObservablePropertiesMetadata.AddRange(
						TypesToObservablePropertiesManagers[type].GetAllObservablePropertiesMetadata()
					);

					return mapFeaturesMetadata;
				})
				.Where(mapFeaturesMetadata => mapFeaturesMetadata.OpenLayersStyle != null);
		}
	}
}