using OSMLSGlobalLibrary;
using OSMLSGlobalLibrary.Modules;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace OSM
{
    class ModulesLibrary
    {
        public Dictionary<string, OSMLSModule> modules = new Dictionary<string, OSMLSModule>();

        public ModulesLibrary(OsmXml rawData, MapObjectsCollection mapObjects)
        {
            foreach (string file in Directory.EnumerateFiles(Constants.ModulesPath, "*.dll"))
            {
                try
                {
                    var DLL = Assembly.LoadFile(Path.GetFullPath(file));

                    foreach (Type type in DLL.GetExportedTypes())
                    {
                        if (type.IsSubclassOf(typeof(OSMLSModule)))
                        {
                            modules[type.Name] = (OSMLSModule)Activator.CreateInstance(type, rawData, modules, mapObjects);
                        }
                    }
                }
                catch (Exception exeption)
                {
                    Console.WriteLine("DLL " + file + " can't be loaded as a module. Inner Exception:\n" + exeption.InnerException);
                    Console.WriteLine("Press Enter to continue...");
                    Console.ReadLine();
                }
            }
        }

    }
}
