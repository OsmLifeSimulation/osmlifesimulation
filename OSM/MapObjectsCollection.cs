using NetTopologySuite.Geometries;
using OSMLSGlobalLibrary;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OSM
{
    public class MapObjectsCollection : InheritanceTreeCollection<Geometry>
    {
        Type itemsType;
        HashSet<Geometry> items = new HashSet<Geometry>();
        Dictionary<Type, MapObjectsCollection> inheritors = new Dictionary<Type, MapObjectsCollection>();

        public MapObjectsCollection()
        {
            itemsType = typeof(Geometry);
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

        public List<(Type type, HashSet<Geometry> mapObjects)> GetTypeItems()
        {
            List<(Type type, HashSet<Geometry>)> typeItems = new List<(Type type, HashSet<Geometry>)>();
            GetTypeItems(typeItems);

            return typeItems;
        }

        void GetTypeItems(List<(Type, HashSet<Geometry>)> typeItems)
        {
            typeItems.Add((itemsType, GetInternal(itemsType)));

            foreach (var inheritor in inheritors)
            {
                inheritor.Value.GetTypeItems(typeItems);
            }
        }

        HashSet<Geometry> GetInternal(Type type)
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

        public override List<Geometry> Get(Type type)
        {
            return GetInternal(type).ToList();
        }

        public override List<T> GetAll<T>()
        {
            return GetAll(typeof(T)).Cast<T>().ToList();
        }

        public override List<Geometry> GetAll(Type type)
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

        public override void Add(Geometry item)
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

        public override void Remove(Geometry item)
        {
            GetInternal(item.GetType()).Remove(item);
        }
    }
}
