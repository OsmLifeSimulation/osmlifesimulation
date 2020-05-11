using OSMLSGlobalLibrary;
using OSMLSGlobalLibrary.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using NetTopologySuite.Geometries;

namespace OSMLS
{
    public class MapObjectsCollection : IInheritanceTreeCollection<Geometry>
    {
        private readonly Type _itemsType;
        private readonly HashSet<Geometry> _items = new HashSet<Geometry>();
        private readonly Dictionary<Type, MapObjectsCollection> _inheritors = new Dictionary<Type, MapObjectsCollection>();

		private readonly object _synchronizationLock = new object();

        public MapObjectsCollection()
        {
            _itemsType = typeof(Geometry);
        }

        private MapObjectsCollection(Type type)
        {
            _itemsType = type;
        }

        private static Type GetFirstInheritor(Type type, Type baseType)
        {
            Type resultType = null;
            for (var current = type; current != baseType; current = current?.BaseType)
                resultType = current;

            return resultType;
        }

        public List<(Type type, List<Geometry> mapObjects)> GetTypeItems()
        {
	        lock (_synchronizationLock)
	        {
				var typeItems = new List<(Type type, HashSet<Geometry>)>();
				GetTypeItems(typeItems);

				return typeItems.Select(x => (x.Item1, x.Item2.ToList())).ToList();
			}
        }

        private void GetTypeItems(List<(Type, HashSet<Geometry>)> typeItems)
        {
            typeItems.Add((_itemsType, GetInternal(_itemsType)));

            foreach (var inheritor in _inheritors)
            {
                inheritor.Value.GetTypeItems(typeItems);
            }
        }

        private HashSet<Geometry> GetInternal(Type type)
        {
	        if (type == _itemsType)
	        {
		        return _items;
	        }

	        var firstInheritor = GetFirstInheritor(type, _itemsType);
	        return _inheritors.ContainsKey(firstInheritor)
		        ? _inheritors[firstInheritor].GetInternal(type)
		        : new HashSet<Geometry>();
        }

        public List<T> Get<T>()
        {
            return Get(typeof(T)).Cast<T>().ToList();
        }

        public List<Geometry> Get(Type type)
        {
	        lock (_synchronizationLock)
	        {
				return GetInternal(type).ToList();
			}
        }

        public List<T> GetAll<T>()
        {
	        return GetAll(typeof(T)).Cast<T>().ToList();
        }

        public List<Geometry> GetAll(Type type)
        {
	        lock (_synchronizationLock)
	        {
		        if (type == _itemsType)
		        {
			        return _items.Concat(_inheritors.SelectMany(x => x.Value.GetAll(x.Key))).ToList();
		        }

		        var firstInheritor = GetFirstInheritor(type, _itemsType);
		        return _inheritors.ContainsKey(firstInheritor)
			        ? _inheritors[firstInheritor].GetAll(type)
			        : new List<Geometry>();
			}
        }

        public void Add(Geometry item)
        {
	        var itemType = item.GetType();

	        lock (_synchronizationLock)
	        {
		        if (itemType == _itemsType)
		        {
			        _items.Add(item);
		        }
		        else
		        {
			        var inheritorType = GetFirstInheritor(itemType, _itemsType);
			        if (!_inheritors.ContainsKey(inheritorType))
			        {
				        _inheritors[inheritorType] = new MapObjectsCollection(inheritorType);
			        }

			        _inheritors[inheritorType].Add(item);
		        }
			}
        }

        public void Remove(Geometry item)
        {
	        lock (_synchronizationLock)
	        {
		        GetInternal(item.GetType()).Remove(item);
	        }
        }
    }
}
