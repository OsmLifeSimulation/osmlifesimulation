using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;
using OSMLSGlobalLibrary;

namespace OSMLS.Model.Objects
{
	public interface IMapObjectsCollection : IInheritanceTreeCollection<Geometry>
	{
		public IDictionary<Type, List<Geometry>> GetTypeItems();
	}
}