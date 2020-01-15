using OSMLSGlobalLibrary.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OSMLSGlobalLibrary.Modules
{
    public abstract class OSMLSModule
    {
        protected Dictionary<string, OSMLSModule> anotherModules { get; }

        protected InheritanceTreeCollection<MapObject> MapObjects { get; }

        protected List<Type> types;

        public OSMLSModule(OsmXml rawData, Dictionary<string, OSMLSModule> modules, InheritanceTreeCollection<MapObject> mapObjects)
        {
            types = Assembly.GetEntryAssembly().GetTypes().Where(x => x.IsClass).ToList();
            anotherModules = modules;

            MapObjects = mapObjects;
        }

        public abstract void Update(long elapsedMilliseconds);

        public object Execute(string methodName, object[] patemeters = null)
        {
            try
            {
                return GetType().GetMethod(methodName).Invoke(this, patemeters);
            }
            catch (Exception)
            {
                Log(string.Format("Some error with {0} method call", methodName));
                return null;
            }

        }

        protected void Log(string msg)
        {
            types.Find(x => x.Name == "Constants").GetMethod("Log").Invoke(null, new object[] { msg, this });
        }

        public object GetProperty(string name)
        {
            return GetType().GetProperty(name).GetValue(this);
        }

        public dynamic GetAnotherModuleAsDynamic(string name)
        {
            return anotherModules[name];
        }
    }
}
