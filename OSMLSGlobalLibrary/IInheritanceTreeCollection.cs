using System;
using System.Collections.Generic;

namespace OSMLSGlobalLibrary
{
    /// <summary>
    /// Inheritance tree collection
    /// </summary>
    /// <typeparam name="TBase"></typeparam>
    public interface IInheritanceTreeCollection<TBase>
    {
        /// <summary>
        /// Gets items of a specific type.
        /// </summary>
        /// <typeparam name="T">Type for which to get items.</typeparam>
        /// <returns>List of items of a specific type.</returns>
        List<T> Get<T>();

        /// <summary>
        /// Gets all items of a specific type.
        /// </summary>
        /// <param name="type">Type for which to get items.</param>
        /// <returns>List of items of a specific type in the base type.</returns>
        List<TBase> Get(Type type);

        /// <summary>
        /// Gets items of a specific type and all inherited types.
        /// </summary>
        /// <typeparam name="T">Type for which to get items.</typeparam>
        /// <returns>List of items of a specific type and all inherited types.</returns>
        List<T> GetAll<T>();

        /// <summary>
        /// Gets items of a specific type and all inherited types.
        /// </summary>
        /// <param name="type">Type for which to get items.</param>
        /// <returns>List of items of a specific type and all inherited types in the base type.</returns>
        List<TBase> GetAll(Type type);

        /// <summary>
        /// Adds the specified element to the collection.
        /// </summary>
        /// <param name="item">The element to add.</param>
        void Add(TBase item);

        /// <summary>
        /// Removes the specified element from the collection.
        /// </summary>
        /// <param name="item">The element to remove.</param>
        void Remove(TBase item);
    }
}
