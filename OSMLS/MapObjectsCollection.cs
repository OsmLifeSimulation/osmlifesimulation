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

        public List<(Type type, HashSet<Geometry> mapObjects)> GetTypeItems()
        {
            List<(Type type, HashSet<Geometry>)> typeItems = new List<(Type type, HashSet<Geometry>)>();
            GetTypeItems(typeItems);

            return typeItems;
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
            return type == _itemsType ? _items : 
                _inheritors[GetFirstInheritor(type, _itemsType)].GetInternal(type);
        }

        public List<T> Get<T>()
        {
            return Get(typeof(T)).Cast<T>().ToList();
        }

        public List<Geometry> Get(Type type)
        {
            return GetInternal(type).ToList();
        }

        public List<T> GetAll<T>()
        {
            return GetAll(typeof(T)).Cast<T>().ToList();
        }

        public List<Geometry> GetAll(Type type)
        {
            return type == _itemsType ?
                _items.Concat(_inheritors.SelectMany(x => x.Value.GetAll(x.Key))).ToList() :
                _inheritors[GetFirstInheritor(type, _itemsType)].GetAll(type);
        }

        public void Add(Geometry item)
        {
            var itemType = item.GetType();

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

        public void Remove(Geometry item)
        {
            GetInternal(item.GetType()).Remove(item);
        }
    }
}
