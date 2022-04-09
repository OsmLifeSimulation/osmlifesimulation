using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using NetTopologySuite.Geometries;

namespace OSMLS.Model.Objects
{
	public class MapObjectsCollection : IMapObjectsCollection
	{
		public event NotifyCollectionChangedEventHandler CollectionChanged = delegate { };

		private readonly Type _ItemsType;
		private readonly HashSet<Geometry> _Items = new();

		private readonly Dictionary<Type, MapObjectsCollection> _Inheritors = new();

		private readonly object _SynchronizationLock = new();

		public MapObjectsCollection()
		{
			_ItemsType = typeof(Geometry);
		}

		private MapObjectsCollection(Type type)
		{
			_ItemsType = type;
		}

		private static Type GetFirstInheritor(Type type, Type baseType)
		{
			Type resultType = null;
			for (var current = type; current != baseType; current = current?.BaseType)
				resultType = current;

			return resultType;
		}

		public IDictionary<Type, List<Geometry>> GetTypeItems()
		{
			lock (_SynchronizationLock)
			{
				var typeItems = new List<(Type type, HashSet<Geometry> items)>();
				GetTypeItems(typeItems);

				return typeItems.ToDictionary(typesToItems =>
						typesToItems.type,
					typesToItems => typesToItems.items.ToList()
				);
			}
		}

		private void GetTypeItems(List<(Type, HashSet<Geometry>)> typeItems)
		{
			typeItems.Add((_ItemsType, GetInternal(_ItemsType)));

			foreach (var inheritor in _Inheritors)
			{
				inheritor.Value.GetTypeItems(typeItems);
			}
		}

		private HashSet<Geometry> GetInternal(Type type)
		{
			if (type == _ItemsType)
			{
				return _Items;
			}

			var firstInheritor = GetFirstInheritor(type, _ItemsType);
			return _Inheritors.ContainsKey(firstInheritor)
				? _Inheritors[firstInheritor].GetInternal(type)
				: new HashSet<Geometry>();
		}

		public List<T> Get<T>()
		{
			return Get(typeof(T)).Cast<T>().ToList();
		}

		public List<Geometry> Get(Type type)
		{
			lock (_SynchronizationLock)
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
			lock (_SynchronizationLock)
			{
				if (type == _ItemsType)
				{
					return _Items.Concat(_Inheritors.SelectMany(x => x.Value.GetAll(x.Key))).ToList();
				}

				var firstInheritor = GetFirstInheritor(type, _ItemsType);
				return _Inheritors.ContainsKey(firstInheritor)
					? _Inheritors[firstInheritor].GetAll(type)
					: new List<Geometry>();
			}
		}

		public void Add(Geometry item)
		{
			var itemType = item.GetType();

			lock (_SynchronizationLock)
			{
				if (itemType == _ItemsType)
				{
					_Items.Add(item);
				}
				else
				{
					var inheritorType = GetFirstInheritor(itemType, _ItemsType);
					if (!_Inheritors.ContainsKey(inheritorType))
					{
						_Inheritors[inheritorType] = new MapObjectsCollection(inheritorType);
					}

					_Inheritors[inheritorType].Add(item);
				}

				CollectionChanged.Invoke(this,
					new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
			}
		}

		public void Remove(Geometry item)
		{
			lock (_SynchronizationLock)
			{
				GetInternal(item.GetType()).Remove(item);

				CollectionChanged.Invoke(this,
					new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
			}
		}
	}
}