using System.Collections.Generic;
using System.Linq;
using OSMLS.Types;
using OSMLS.Types.Model;

namespace OSMLS.Model.Modules
{
	public class ModulesLibrary : IModulesLibrary
	{
		public ModulesLibrary(IInjectedTypesProvider injectedTypesProvider)
		{
			_InjectedTypesProvider = injectedTypesProvider;
		}

		private readonly IInjectedTypesProvider _InjectedTypesProvider;

		public IEnumerable<IInjectedModuleType> ModulesTypes =>
			_InjectedTypesProvider.GetTypes().OfType<IInjectedModuleType>();

		public IInjectedModuleType GetModuleType(string name) => ModulesTypes.First(type => type.FullName == name);
	}
}