using System.Collections.Generic;
using OSMLS.Types.Model;

namespace OSMLS.Model.Modules
{
	public interface IModulesLibrary
	{
		public IEnumerable<IInjectedModuleType> ModulesTypes { get; }

		public IInjectedModuleType GetModuleType(string name);
	}
}