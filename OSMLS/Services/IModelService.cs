using System;
using System.Collections.Generic;
using Microsoft.Extensions.Hosting;

namespace OSMLS.Services
{
	public interface IModelService : IHostedService, IDisposable
	{
		public IList<Type> ModulesTypes { get; }

		public bool IsPaused { get; set; }

		public bool IsStopped { get; }
	}
}