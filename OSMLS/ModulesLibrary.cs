using OSMLSGlobalLibrary.Modules;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace OSMLS
{
    internal class ModulesLibrary
    {
        public Dictionary<Type, OSMLSModule> Modules { get; } = new Dictionary<Type, OSMLSModule>();

        public ModulesLibrary(string osmFilePath, MapObjectsCollection mapObjects)
        {
            var assemblies = new List<Assembly>();
            foreach (var file in Directory.EnumerateFiles(Constants.ModulesDirectoryPath, "*.dll"))
            {
                try
                {
                    assemblies.Add(AssemblyLoadContext.Default.LoadFromAssemblyPath(Path.GetFullPath(file)));
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"DLL {file} can't be loaded. Exception:\n{exception}");
                }
            }

            var moduleTypesToOrder = assemblies
                .SelectMany(a => a.GetExportedTypes())
                .Where(type => type.IsSubclassOf(typeof(OSMLSModule)))
                .ToDictionary(type => type, type =>
                    {
                        var initializationOrderAttribute = (CustomInitializationOrderAttribute)type
                            .GetCustomAttributes(typeof(CustomInitializationOrderAttribute), false)
                            .FirstOrDefault();

                        var initializationOrder = initializationOrderAttribute?.InitializationOrder ?? 0;

                        return initializationOrder;
                    }
                );

            Console.WriteLine(moduleTypesToOrder.Count == 0 ? "No modules found." : "Starting modules initialization in order.");

            foreach (var type in moduleTypesToOrder.Keys)
            {
                // Writes initialization order.
                Console.Write($"{moduleTypesToOrder[type]}: ");

                try
                {
                    var moduleInstance = (OSMLSModule)Activator.CreateInstance(type);
                    moduleInstance.Initialize(osmFilePath, Modules, mapObjects);
                    Modules[type] = moduleInstance;
                    Console.WriteLine($"{type.Name} initialized successfully.");
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"Module {type.Name} can't be loaded. Exception:\n{exception}");
                }
            }
        }
    }
}
