using System;
using System.Reflection;
using OSMLSGlobalLibrary.Modules;

namespace OSMLS.Types.Model
{
	public class InjectedModuleType : InjectedType, IInjectedModuleType
	{
		public InjectedModuleType(Type systemType) : base(systemType)
		{
		}

		public IModule CreateInstance() => (OSMLSModule)Activator.CreateInstance(SystemType);

		private CustomInitializationOrderAttribute GetCustomInitializationOrderAttribute() =>
			(CustomInitializationOrderAttribute)SystemType.GetCustomAttribute(
				typeof(CustomInitializationOrderAttribute)
			);

		public double GetInitializationOrder() => GetCustomInitializationOrderAttribute()?.InitializationOrder ?? 0;
	}
}