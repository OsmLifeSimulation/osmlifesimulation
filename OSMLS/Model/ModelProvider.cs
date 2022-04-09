using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NetTopologySuite.Geometries;
using OSMLS.Types.Model;
using OSMLSGlobalLibrary;
using OSMLSGlobalLibrary.Modules;

namespace OSMLS.Model
{
	public class ModelProvider : IModelProvider
	{
		public ModelProvider(IInheritanceTreeCollection<Geometry> mapObjects)
		{
			_MapObjects = mapObjects;
		}

		public IList<IInjectedModuleType> ModulesTypes { get; } = new List<IInjectedModuleType>();

		public IImmutableDictionary<IInjectedModuleType, IModule> TypesToInitializedModules { get; private set; } =
			ImmutableDictionary<IInjectedModuleType, IModule>.Empty;

		private readonly string _OsmFilePath = $"{AppContext.BaseDirectory}/map.osm";

		public bool IsPaused { get; set; }

		public bool IsStopped { get; private set; } = true;

		private Timer _Timer;

		private Stopwatch TimeNow { get; } = new();

		private readonly IInheritanceTreeCollection<Geometry> _MapObjects;

		public event INotifyModuleInitialized.ModuleInitializedEventHandler ModuleInitialized = delegate { };

		private void InitializeModules()
		{
			TypesToInitializedModules = ModulesTypes.ToDictionary(
				moduleType => moduleType,
				moduleType => moduleType.CreateInstance()
			).ToImmutableDictionary();

			var moduleTypesToOrder = TypesToInitializedModules.Keys.ToDictionary(
				type => type,
				type => type.GetInitializationOrder()
			);

			Console.WriteLine(moduleTypesToOrder.Count == 0
				? "No modules found."
				: "Starting modules initialization in order.");

			foreach (var type in moduleTypesToOrder.OrderBy(x => x.Value).Select(x => x.Key))
			{
				// Writes initialization order.
				Console.WriteLine($"{moduleTypesToOrder[type]}: {type.FullName} initialization started.");

				var module = TypesToInitializedModules[type];
				module.Initialize(
					_OsmFilePath,
					TypesToInitializedModules.ToImmutableDictionary(
						pair => pair.Key.SystemType,
						pair => pair.Value
					),
					_MapObjects
				);
				ModuleInitialized.Invoke(this, new INotifyModuleInitialized.ModuleInitializedEventArgs(module));
			}
		}

		public Task StartAsync(CancellationToken cancellationToken)
		{
			if (!IsStopped)
				throw new InvalidOperationException("Model is already started.");

			InitializeModules();
			IsStopped = false;
			_Timer = new Timer(Update, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));

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
			TypesToInitializedModules = ImmutableDictionary<IInjectedModuleType, IModule>.Empty;

			return Task.CompletedTask;
		}

		public void Dispose() => _Timer?.Dispose();
	}
}