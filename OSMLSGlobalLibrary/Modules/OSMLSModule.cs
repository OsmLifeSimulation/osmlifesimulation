using System;
using OSMLSGlobalLibrary.Map;
using System.Collections.Generic;

namespace OSMLSGlobalLibrary.Modules
{
    public abstract class OSMLSModule
    {
        protected string OsmFilePath { get; private set; }

        private Dictionary<Type, OSMLSModule> _allModules;

        protected InheritanceTreeCollection<MapObject> MapObjects { get; private set; }

        private bool _isInitialized;

        private readonly object _initializationLock = new object();

        public void Initialize(string osmFilePath, Dictionary<Type, OSMLSModule> modules, InheritanceTreeCollection<MapObject> mapObjects)
        {
            lock (_initializationLock)
            {
                if (_isInitialized)
                {
                    throw new InvalidOperationException("Module already initialized.");
                }

                OsmFilePath = osmFilePath;
                _allModules = modules;
                MapObjects = mapObjects;

                Initialize();

                _isInitialized = true;
            }
        }

        protected abstract void Initialize();

        public abstract void Update(long elapsedMilliseconds);

        public TModule GetModule<TModule>() where TModule : OSMLSModule
        {
            return _allModules[typeof(TModule)] as TModule;
        }
    }
}
