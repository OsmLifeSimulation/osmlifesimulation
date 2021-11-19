using System;
using OSMLSGlobalLibrary.Modules;

namespace OSMLS.Services
{
	public interface INotifyModuleInitialized
	{
		class ModuleInitializedEventArgs : EventArgs
		{
			public OSMLSModule Module { get; }

			public ModuleInitializedEventArgs(OSMLSModule module)
			{
				Module = module;
			}
		}

		public delegate void ModuleInitializedEventHandler(object sender, ModuleInitializedEventArgs eventArgs);

		event ModuleInitializedEventHandler ModuleInitialized;
	}
}