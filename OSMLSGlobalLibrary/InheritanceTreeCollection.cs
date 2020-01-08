using System;
using System.Collections.Generic;

namespace OSMLSGlobalLibrary
{
    /// <summary>
    /// Inheritance tree collection
    /// </summary>
    /// <typeparam name="BaseType"></typeparam>
    public abstract class InheritanceTreeCollection<BaseType>
    {
        /// <summary>
        /// Gets items of a specific type.
        /// </summary>
        /// <typeparam name="T">Type for which to get items.</typeparam>
        /// <returns>List of items of a specific type.</returns>
        public abstract List<T> Get<T>();

        /// <summary>
        /// Gets all items of a specific type.
        /// </summary>
        /// <param name="type">Type for which to get items.</param>
        /// <returns>List of items of a specific type in the base type.</returns>
        public abstract List<BaseType> Get(Type type);

        /// <summary>
        /// Gets items of a specific type and all inherited types.
        /// </summary>
        /// <typeparam name="T">Type for which to get items.</typeparam>
        /// <returns>List of items of a specific type and all inherited types.</returns>
        public abstract List<T> GetAll<T>();

        /// <summary>
        /// Gets items of a specific type and all inherited types.
        /// </summary>
        /// <param name="type">Type for which to get items.</param>
        /// <returns>List of items of a specific type and all inherited types in the base type.</returns>
        public abstract List<BaseType> GetAll(Type type);

        /// <summary>
        /// Adds the specified element to the collection.
        /// </summary>
        /// <param name="item">The element to add.</param>
        public abstract void Add(BaseType item);

        /// <summary>
        /// Removes the specified element from the collection.
        /// </summary>
        /// <param name="item">The element to remove.</param>
        public abstract void Remove(BaseType item);
    }
}
