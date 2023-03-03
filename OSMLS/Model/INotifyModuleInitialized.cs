using System;
using OSMLSGlobalLibrary.Modules;

namespace OSMLS.Model
{
	public interface INotifyModuleInitialized
	{
		class ModuleInitializedEventArgs : EventArgs
		{
			public IModule Module { get; }

			public ModuleInitializedEventArgs(IModule module)
			{
				Module = module;
			}
		}

		public delegate void ModuleInitializedEventHandler(object sender, ModuleInitializedEventArgs eventArgs);

		event ModuleInitializedEventHandler ModuleInitialized;
	}
}