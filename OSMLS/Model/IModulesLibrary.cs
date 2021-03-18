using System;
using System.Collections.Generic;
using System.IO;

namespace OSMLS.Model
{
	public interface IModulesLibrary
	{
		public IEnumerable<Type> ModulesTypes { get; }

		public Type GetType(string name);

		public void LoadModules(Stream assemblyStream);
	}
}