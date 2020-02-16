using OSMLSGlobalLibrary;
using OSMLSGlobalLibrary.Map;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OSMLS
{
    public class MapObjectsCollection : InheritanceTreeCollection<MapObject>
    {
        private readonly Type _itemsType;
        private readonly HashSet<MapObject> _items = new HashSet<MapObject>();
        private readonly Dictionary<Type, MapObjectsCollection> _inheritors = new Dictionary<Type, MapObjectsCollection>();

        public MapObjectsCollection()
        {
            _itemsType = typeof(MapObject);
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

        public List<(Type type, HashSet<MapObject> mapObjects)> GetTypeItems()
        {
            List<(Type type, HashSet<MapObject>)> typeItems = new List<(Type type, HashSet<MapObject>)>();
            GetTypeItems(typeItems);

            return typeItems;
        }

        private void GetTypeItems(List<(Type, HashSet<MapObject>)> typeItems)
        {
            typeItems.Add((_itemsType, GetInternal(_itemsType)));

            foreach (var inheritor in _inheritors)
            {
                inheritor.Value.GetTypeItems(typeItems);
            }
        }

        private HashSet<MapObject> GetInternal(Type type)
        {
            return type == _itemsType ? _items : 
                _inheritors[GetFirstInheritor(type, _itemsType)].GetInternal(type);
        }

        public override List<T> Get<T>()
        {
            return Get(typeof(T)).Cast<T>().ToList();
        }

        public override List<MapObject> Get(Type type)
        {
            return GetInternal(type).ToList();
        }

        public override List<T> GetAll<T>()
        {
            return GetAll(typeof(T)).Cast<T>().ToList();
        }

        public override List<MapObject> GetAll(Type type)
        {
            return type == _itemsType ?
                _items.Concat(_inheritors.SelectMany(x => x.Value.GetAll(x.Key))).ToList() :
                _inheritors[GetFirstInheritor(type, _itemsType)].GetAll(type);
        }

        public override void Add(MapObject item)
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

        public override void Remove(MapObject item)
        {
            GetInternal(item.GetType()).Remove(item);
        }
    }
}
