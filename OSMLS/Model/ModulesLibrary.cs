using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using OSMLSGlobalLibrary.Modules;

namespace OSMLS.Model
{
	public class ModulesLibrary
	{
		public List<Type> ModulesTypes { get; } = new();

		public static Type GetType(string name) => AssemblyLoadContext.Default.Assemblies
			.Select(assembly => assembly.GetType(name)).First(type => type != null);

		public void LoadModules(Stream assemblyStream)
		{
			var assembly = AssemblyLoadContext.Default.LoadFromStream(assemblyStream);

			ModulesTypes.AddRange(
				assembly
				.GetExportedTypes()
				.Where(type => type.IsSubclassOf(typeof(OSMLSModule)))
			);
		}
	}
}