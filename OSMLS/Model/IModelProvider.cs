using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.Extensions.Hosting;
using OSMLS.Types.Model;
using OSMLSGlobalLibrary.Modules;

namespace OSMLS.Model
{
	public interface IModelProvider : INotifyModuleInitialized, IHostedService, IDisposable
	{
		public IList<IInjectedModuleType> ModulesTypes { get; }

		public IImmutableDictionary<IInjectedModuleType, IModule> TypesToInitializedModules { get; }

		public bool IsPaused { get; set; }

		public bool IsStopped { get; }
	}
}