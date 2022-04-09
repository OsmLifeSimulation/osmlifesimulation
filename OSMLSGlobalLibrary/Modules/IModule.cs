using System;
using System.Collections.Immutable;
using System.ComponentModel;
using NetTopologySuite.Geometries;

namespace OSMLSGlobalLibrary.Modules
{
	public interface IModule : INotifyPropertyChanged
	{
		void Initialize(
			string osmFilePath,
			IImmutableDictionary<Type, IModule> modules,
			IInheritanceTreeCollection<Geometry> mapObjects
		);

		void Update(long elapsedMilliseconds);
	}
}