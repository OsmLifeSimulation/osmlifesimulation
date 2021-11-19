using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NetTopologySuite.Geometries;
using OSMLSGlobalLibrary;
using OSMLSGlobalLibrary.Modules;

namespace OSMLS.Services
{
	public class ModelService : IModelService
	{
		public ModelService(IInheritanceTreeCollection<Geometry> mapObjects)
		{
			_MapObjects = mapObjects;
		}

		public IList<Type> ModulesTypes { get; } = new List<Type>();

		public IImmutableDictionary<Type, OSMLSModule> TypesToInitializedModules { get; private set; } = 
			ImmutableDictionary<Type, OSMLSModule>.Empty;

		private readonly string _OsmFilePath = $"{AppContext.BaseDirectory}/map.osm";

		public bool IsPaused { get; set; }

		public bool IsStopped { get; private set; }

		private Timer _Timer;

		private Stopwatch TimeNow { get; } = new();

		private readonly IInheritanceTreeCollection<Geometry> _MapObjects;

		public event INotifyModuleInitialized.ModuleInitializedEventHandler ModuleInitialized = delegate { };

		private void InitializeModules()
		{
			TypesToInitializedModules = ModulesTypes.ToDictionary(
				moduleType => moduleType,
				moduleType => (OSMLSModule) Activator.CreateInstance(moduleType)
			).ToImmutableDictionary();

			var moduleTypesToOrder = TypesToInitializedModules.Keys.ToDictionary(type => type, type =>
				{
					var initializationOrderAttribute = (CustomInitializationOrderAttribute) type
						.GetCustomAttributes(typeof(CustomInitializationOrderAttribute), false)
						.FirstOrDefault();

					var initializationOrder =
						(initializationOrderAttribute ?? new CustomInitializationOrderAttribute())
						.InitializationOrder;

					return initializationOrder;
				}
			);

			Console.WriteLine(moduleTypesToOrder.Count == 0
				? "No modules found."
				: "Starting modules initialization in order.");

			foreach (var type in moduleTypesToOrder.OrderBy(x => x.Value).Select(x => x.Key))
			{
				// Writes initialization order.
				Console.WriteLine($"{moduleTypesToOrder[type]}: {type.Name} initialization started.");

				var module = TypesToInitializedModules[type];
				module.Initialize(_OsmFilePath, TypesToInitializedModules, _MapObjects);
				ModuleInitialized.Invoke(this, new INotifyModuleInitialized.ModuleInitializedEventArgs(module));
			}
		}

		public Task StartAsync(CancellationToken cancellationToken)
		{
			InitializeModules();
			IsStopped = false;
			_Timer = new Timer(Update, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(50));

			TimeNow.Start();

			return Task.CompletedTask;
		}

		private void Update(object state)
		{
			foreach (var module in TypesToInitializedModules.Values)
			{
				if (IsPaused)
					return;

				module.Update(TimeNow.ElapsedMilliseconds);
			}
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			_Timer?.Change(Timeout.Infinite, 0);
			IsStopped = true;

			_MapObjects.GetAll<Geometry>().ForEach(geometry => _MapObjects.Remove(geometry));
			TypesToInitializedModules = ImmutableDictionary<Type, OSMLSModule>.Empty;

			return Task.CompletedTask;
		}

		public void Dispose() => _Timer?.Dispose();
	}
}