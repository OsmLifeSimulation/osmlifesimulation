using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.Extensions.Hosting;
using OSMLSGlobalLibrary.Modules;

namespace OSMLS.Services
{
	public interface IModelService : INotifyModuleInitialized, IHostedService, IDisposable
	{
		public IList<Type> ModulesTypes { get; }

		public IImmutableDictionary<Type, OSMLSModule> TypesToInitializedModules { get; }

		public bool IsPaused { get; set; }

		public bool IsStopped { get; }
	}
}