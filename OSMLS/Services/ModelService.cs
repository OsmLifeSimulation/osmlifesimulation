using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using NetTopologySuite.Geometries;
using OSMLSGlobalLibrary;
using OSMLSGlobalLibrary.Modules;

namespace OSMLS.Services
{
	public class ModelService : IHostedService, IDisposable
	{
		public ModelService(IInheritanceTreeCollection<Geometry> mapObjects)
		{
			_MapObjects = mapObjects;
		}

		public List<Type> Modules { get; } = new();

		private Dictionary<Type, OSMLSModule> InternalModules { get; set; }

		private readonly string _OsmFilePath = $"{AppContext.BaseDirectory}/map.osm";

		public bool IsPaused { get; set; }
		public bool IsStopped { get; private set; }

		private Timer _Timer;

		private Stopwatch TimeNow { get; } = new();

		private readonly IInheritanceTreeCollection<Geometry> _MapObjects;

		private void InitializeModules()
		{
			InternalModules = Modules.ToDictionary(
				moduleType => moduleType,
				moduleType => (OSMLSModule) Activator.CreateInstance(moduleType)
			);

			var moduleTypesToOrder = InternalModules.Keys.ToDictionary(type => type, type =>
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

				InternalModules[type].Initialize(_OsmFilePath, InternalModules, _MapObjects);
			}
		}

		public Task StartAsync(CancellationToken cancellationToken)
		{
			IsStopped = false;
			InitializeModules();
			_Timer = new Timer(Update, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(50));

			TimeNow.Start();

			return Task.CompletedTask;
		}

		private void Update(object state)
		{
			foreach (var module in InternalModules.Values)
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

			return Task.CompletedTask;
		}

		public void Dispose() => _Timer?.Dispose();
	}
}