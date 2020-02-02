using OSMLSGlobalLibrary;
using OSMLSGlobalLibrary.Modules;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace OSM
{
    class ModulesLibrary
    {
        public Dictionary<string, OSMLSModule> modules = new Dictionary<string, OSMLSModule>();

        public ModulesLibrary(OsmXml rawData, MapObjectsCollection mapObjects)
        {
            var assemblies = new List<Assembly>();
            foreach (string file in Directory.EnumerateFiles(Constants.ModulesPath, "*.dll"))
            {
                try
                {
                    assemblies.Add(AssemblyLoadContext.Default.LoadFromAssemblyPath(Path.GetFullPath(file)));
                }
                catch (Exception exeption)
                {
                    Console.WriteLine($"DLL {file} can't be loaded. Exception:\n{exeption}");
                }
            }

            var moduleTypes = assemblies
                .SelectMany(a => a.GetExportedTypes())
                .Where(type => type.IsSubclassOf(typeof(OSMLSModule)))
                .OrderBy(type => 
                    ((CustomInitializationOrderAttribute)type.GetCustomAttributes(typeof(CustomInitializationOrderAttribute), false).FirstOrDefault())
                        ?.InitializationOrder ?? 0
                );

            foreach (var type in moduleTypes)
            {
                try
                {
                    modules[type.Name] = (OSMLSModule)Activator.CreateInstance(type, rawData, modules, mapObjects);
                }
                catch (Exception exeption)
                {
                    Console.WriteLine($"Module {type.Name} can't be loaded. Exception:\n{exeption}");
                }
            }
        }

    }
}
