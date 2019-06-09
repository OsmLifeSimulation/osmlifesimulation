using Microsoft.Xna.Framework;
using OSMGlobalLibrary.SuperModule;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace OSM
{
    class ModulesLibrary
    {
        public Dictionary<string, OSMModule> modules = new Dictionary<string, OSMModule>();
        public List<(List<Point>, string)> DrawableData {
            get
            {
                return modules.SelectMany(m => m.Value.DrawableData()).ToList();
            }
        }

        public ModulesLibrary(OSMData data)
        {
            foreach (string file in Directory.EnumerateFiles(Constants.ModulesPath, "*.dll"))
            {
                try
                {
                    var DLL = Assembly.LoadFile(Path.GetFullPath(file));

                    foreach (Type type in DLL.GetExportedTypes())
                    {
                        if (type.IsSubclassOf(typeof(OSMModule)))
                        {
                            modules[Regex.Match(type.FullName.Split('.').Last(), @"(.+)" + Constants.ModuleIdentifier).Groups[1].Value]= (OSMModule)Activator.CreateInstance(type, data, modules);
                        }
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("DLL " + file + " can't be loaded as a module");
                }
            }
        }

    }
}
