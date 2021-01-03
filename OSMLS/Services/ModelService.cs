using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using OSMLS.Model;

namespace OSMLS.Services
{
	internal class ModelService : IHostedService, IDisposable
	{
		public ModelService(ModulesLibrary modulesLibrary)
		{
			_ModulesLibrary = modulesLibrary;
		}

		private readonly ModulesLibrary _ModulesLibrary;
		private Timer _Timer;

		private Stopwatch TimeNow { get; } = new();

		public Task StartAsync(CancellationToken cancellationToken)
		{
			_Timer = new Timer(Update, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(50));

			TimeNow.Start();

			return Task.CompletedTask;
		}

		private void Update(object state)
		{
			foreach (var module in _ModulesLibrary.Modules)
			{
				module.Value.Update(TimeNow.ElapsedMilliseconds);
			}
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			_Timer?.Change(Timeout.Infinite, 0);

			return Task.CompletedTask;
		}

		public void Dispose() => _Timer?.Dispose();
	}
}