using NetTopologySuite.Geometries;
using OSMLSGlobalLibrary;
using OSMLSGlobalLibrary.Map;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OSM
{
    public class MapObjectsCollection : InheritanceTreeCollection<MapObject>
    {
        Type itemsType;
        HashSet<MapObject> items = new HashSet<MapObject>();
        Dictionary<Type, MapObjectsCollection> inheritors = new Dictionary<Type, MapObjectsCollection>();

        public MapObjectsCollection()
        {
            itemsType = typeof(MapObject);
        }

        MapObjectsCollection(Type type)
        {
            itemsType = type;
        }

        Type GetFirstInheritor(Type type, Type baseType)
        {
            Type resultType = null;
            for (var current = type; current != baseType; current = current.BaseType)
                resultType = current;

            return resultType;
        }

        public List<(Type type, HashSet<MapObject> mapObjects)> GetTypeItems()
        {
            List<(Type type, HashSet<MapObject>)> typeItems = new List<(Type type, HashSet<MapObject>)>();
            GetTypeItems(typeItems);

            return typeItems;
        }

        void GetTypeItems(List<(Type, HashSet<MapObject>)> typeItems)
        {
            typeItems.Add((itemsType, GetInternal(itemsType)));

            foreach (var inheritor in inheritors)
            {
                inheritor.Value.GetTypeItems(typeItems);
            }
        }

        HashSet<MapObject> GetInternal(Type type)
        {
            if (type == itemsType)
            {
                return items;
            }
            else
            {
                return inheritors[GetFirstInheritor(type, itemsType)].GetInternal(type);
            }
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
            if (type == itemsType)
            {
                return items.Concat(inheritors.SelectMany(x => x.Value.GetAll(x.Key))).ToList();
            }
            else
            {
                return inheritors[GetFirstInheritor(type, itemsType)].GetAll(type);
            }
        }

        public override void Add(MapObject item)
        {
            var itemType = item.GetType();

            if (itemType == itemsType)
            {
                items.Add(item);
            }
            else
            {
                var inheritorType = GetFirstInheritor(itemType, itemsType);
                if (!inheritors.ContainsKey(inheritorType))
                {
                    inheritors[inheritorType] = new MapObjectsCollection(inheritorType);
                }

                inheritors[inheritorType].Add(item);
            }
        }

        public override void Remove(MapObject item)
        {
            GetInternal(item.GetType()).Remove(item);
        }
    }
}
