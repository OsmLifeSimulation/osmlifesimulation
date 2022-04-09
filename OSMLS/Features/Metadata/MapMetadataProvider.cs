using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using OSMLS.Features.Properties;
using OSMLS.Map;
using OSMLS.Types;
using OSMLS.Types.Model;
using OSMLS.Types.Model.Properties;

namespace OSMLS.Features.Metadata
{
	public class MapMetadataProvider : IMapFeaturesMetadataProvider, IObservablePropertiesMetadataProvider
	{
		public MapMetadataProvider(IInjectedTypesProvider injectedTypesProvider)
		{
			_InjectedTypesProvider = injectedTypesProvider;

			TypesToObservablePropertiesManagers = _InjectedTypesProvider.GetTypes()
				.ToDictionary(
					type => type.SystemType,
					type =>
						new ObservablePropertiesManager(InjectedProperty.FromType(type)) as IObservablePropertiesManager
				).ToImmutableDictionary();

			var mapFeaturesMetadataSubject = new Subject<MapFeaturesMetadata>();
			MapFeaturesMetadataObservable = mapFeaturesMetadataSubject.AsObservable();
			_InjectedTypesProvider.TypeAdded += (_, args) =>
			{
				var type = args.Type;

				TypesToObservablePropertiesManagers = TypesToObservablePropertiesManagers.Add(
					type.SystemType,
					new ObservablePropertiesManager(InjectedProperty.FromType(type))
				);

				GetMapFeaturesMetadata(new[] { type })
					.ToList()
					.ForEach(mapFeaturesMetadata => mapFeaturesMetadataSubject.OnNext(mapFeaturesMetadata));
			};
		}

		private readonly IInjectedTypesProvider _InjectedTypesProvider;

		public IEnumerable<MapFeaturesMetadata> GetMapFeaturesMetadata() =>
			GetMapFeaturesMetadata(_InjectedTypesProvider.GetTypes());

		public IImmutableDictionary<Type, IObservablePropertiesManager>
			TypesToObservablePropertiesManagers { get; private set; }

		public IObservable<MapFeaturesMetadata> MapFeaturesMetadataObservable { get; }

		private IEnumerable<MapFeaturesMetadata> GetMapFeaturesMetadata(IEnumerable<IInjectedType> types)
		{
			return types
				.Where(type => type is IInjectedModuleType or IInjectedActorType)
				.Select(type =>
				{
					var mapFeaturesMetadata = new MapFeaturesMetadata { TypeFullName = type.FullName };

					if (type is IInjectedActorType { IsVisible: true } actorType)
						mapFeaturesMetadata.OpenLayersStyle = actorType.GetCustomStyle();

					mapFeaturesMetadata.ObservablePropertiesMetadata.AddRange(
						TypesToObservablePropertiesManagers[type.SystemType].GetAllObservablePropertiesMetadata()
					);

					return mapFeaturesMetadata;
				});
		}
	}
}