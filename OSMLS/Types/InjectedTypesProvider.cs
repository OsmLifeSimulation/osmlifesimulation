using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using MoreLinq;
using NetTopologySuite.Geometries;
using OSMLS.Types.Model;
using OSMLSGlobalLibrary.Modules;
using OSMLSGlobalLibrary.Observable.Geometries.Actor;

namespace OSMLS.Types
{
	public class InjectedTypesProvider : IInjectedTypesProvider, IAssemblyLoader
	{
		public InjectedTypesProvider(AssemblyLoadContext assemblyLoadContext)
		{
			_AssemblyLoadContext = assemblyLoadContext;
		}

		private readonly AssemblyLoadContext _AssemblyLoadContext;

		private bool TryComposeInjectedType(Type type, out IInjectedType composedType)
		{
			if (type.IsSubclassOf(typeof(OSMLSModule)))
			{
				composedType = new InjectedModuleType(type);
				return true;
			}

			if (type.IsSubclassOf(typeof(Geometry)) && type.IsAssignableTo(typeof(IActor)))
			{
				composedType = new InjectedActorType(type);
				return true;
			}

			composedType = null;
			return false;
		}

		public IEnumerable<IInjectedType> GetTypes()
		{
			foreach (var assembly in _AssemblyLoadContext.Assemblies)
			foreach (var type in assembly.GetTypes())
				if (TryComposeInjectedType(type, out var injectedType))
					yield return injectedType;
		}

		public event IInjectedTypesProvider.TypeAddedEventHandler TypeAdded = delegate { };

		public void LoadAssembly(Stream assemblyStream)
		{
			_AssemblyLoadContext.LoadFromStream(assemblyStream)
				.GetTypes()
				.ForEach(type =>
				{
					if (TryComposeInjectedType(type, out var injectedType))
						TypeAdded.Invoke(this, new IInjectedTypesProvider.TypeAddedEventArgs(injectedType));
				});
		}
	}
}