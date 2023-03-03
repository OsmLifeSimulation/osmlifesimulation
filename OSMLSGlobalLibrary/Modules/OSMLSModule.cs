using System;
using System.Collections.Immutable;
using System.ComponentModel;
using NetTopologySuite.Geometries;

namespace OSMLSGlobalLibrary.Modules
{
	/// <summary>
	/// Module class. All modules must be inherited from this class to interact with the main program.
	/// </summary>
	public abstract class OSMLSModule : IModule
	{
		public virtual event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Path to the OSM XML file. It is agreed that within the map section described in this file, modeling should take place.
		/// </summary>
		protected string OsmFilePath { get; private set; }

		/// <summary>
		/// A dictionary containing modules by type.
		/// </summary>
		private IImmutableDictionary<Type, IModule> _allModules;

		/// <summary>
		/// A collection containing all map objects. Objects from this collection are displayed in browser clients. This collection is one for all modules.
		/// </summary>
		protected IInheritanceTreeCollection<Geometry> MapObjects { get; private set; }

		/// <summary>
		/// A flag indicating that the module has already been initialized.
		/// </summary>
		private bool _isInitialized;

		/// <summary>
		/// Provides synchronization context for initialization.
		/// </summary>
		private readonly object _initializationLock = new object();

		/// <summary>
		/// External initialization method. Sets the context of the main program for the module.
		/// </summary>
		/// <param name="osmFilePath">Path to the OSM XML file.</param>
		/// <param name="modules">A dictionary containing modules by type.</param>
		/// <param name="mapObjects">A collection containing all map objects.</param>
		public void Initialize(string osmFilePath, IImmutableDictionary<Type, IModule> modules,
			IInheritanceTreeCollection<Geometry> mapObjects)
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

		/// <summary>
		/// Internal initialization method. In this method, it is necessary to describe the initialization logic of the inherited module.
		/// </summary>
		protected abstract void Initialize();

		/// <summary>
		/// Update method. In this method, it is necessary to describe the update logic of the inherited module.
		/// </summary>
		/// <param name="elapsedMilliseconds">The number of milliseconds since the start of the main program.</param>
		public abstract void Update(long elapsedMilliseconds);

		/// <summary>
		/// Gets module instance by its type.
		/// </summary>
		/// <typeparam name="TModule">Module type.</typeparam>
		/// <returns>Found module instance.</returns>
		protected TModule GetModule<TModule>() where TModule : OSMLSModule
		{
			return _allModules[typeof(TModule)] as TModule;
		}
	}
}