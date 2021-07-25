using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using OSMLSGlobalLibrary.Modules;

namespace OSMLS.Model
{
	public class ModulesLibrary : IModulesLibrary, INotifyAssemblyAdded
	{
		public event INotifyAssemblyAdded.AssemblyAddedEventHandler AssemblyAdded = delegate { };

		public IEnumerable<Type> ModulesTypes { get; private set; } = new List<Type>();

		public Type GetType(string name) => AssemblyLoadContext.Default.Assemblies
			.Select(assembly => assembly.GetType(name)).First(type => type != null);

		public void LoadModules(Stream assemblyStream)
		{
			var assembly = AssemblyLoadContext.Default.LoadFromStream(assemblyStream);

			var modulesTypes = assembly
				.GetExportedTypes()
				.Where(type => type.IsSubclassOf(typeof(OSMLSModule)));

			ModulesTypes = ModulesTypes.Concat(modulesTypes);

			AssemblyAdded.Invoke(this, new INotifyAssemblyAdded.AssemblyAddedEventArgs(assembly));
		}
	}
}