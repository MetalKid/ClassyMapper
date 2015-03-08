#region << Usings >>

using System;
using System.Collections.Generic;

#endregion

namespace ClassyMapper.Interfaces
{
    /// <summary>
    /// This interface defines all public methods ClassyMapper exposes in case this needs to be IoC injected.
    /// </summary>
    public interface IClassyMapper
    {

        #region << Properties >>

        /// <summary>
        /// Gets the configuration data on how the objects will be mapped.
        /// </summary>
        IClassyMapperConfig Config { get; }

        #endregion

        #region << Methods >>

        /// <summary>
        /// Registers a method to call to do custom mapping when working with a specific TFrom -> TTo combination.
        /// </summary>
        /// <typeparam name="TFrom">The type of object being mapped from.</typeparam>
        /// <typeparam name="TTo">The type of object being mapped to.</typeparam>
        /// <param name="method">The method to invoke.</param>
        /// <returns>Instance of this ClassyMapper for chaining.</returns>
        IClassyMapper RegisterCustomMap<TFrom, TTo>(Action<TFrom, TTo> method);

        /// <summary>
        /// Registers a function that returns what objects to map from for a given TFrom object.
        /// This allows a user to flatten a hierarchy.
        /// </summary>
        /// <typeparam name="TFrom">The type of object being mapped from.</typeparam>
        /// <param name="func">The function to invoke when working with a TFrom object.</param>
        /// <returns>Instance of this ClassyMapper for chaining.</returns>
        IClassyMapper RegisterFromObjects<TFrom>(Func<TFrom, object[]> func);

        /// <summary>
        /// Registers a method to call when creating a new TTo type object given the TFrom data.
        /// </summary>
        /// <typeparam name="TFrom">The type of object being mapped from.</typeparam>
        /// <typeparam name="TTo">The type of object being created.</typeparam>
        /// <param name="method">The function to invoke when creating a new TTo from a TFrom mapping.</param>
        /// <returns>New instance of TTo.</returns>
        IClassyMapper RegisterConstructor<TFrom, TTo>(Func<TFrom, TTo> method);

        /// <summary>
        /// Fully maps a list of source object of type TFrom to an IList of TTo type.
        /// </summary>
        /// <typeparam name="TFrom">The type of object being mapped from.</typeparam>
        /// <typeparam name="TTo">The type of object being mapped to.</typeparam>
        /// <param name="fromObjects">The list of objects being mapped from.</param>
        /// <returns>IList of type TTo.</returns>
        IList<TTo> MapToList<TTo, TFrom>(IEnumerable<TFrom> fromObjects);

        /// <summary>
        /// Maps a single source object to an object of type TTo.
        /// </summary>
        /// <typeparam name="TTo">The type of object being mapped to.</typeparam>
        /// <param name="from">The object being mapped from.</param>
        /// <returns></returns>
        TTo Map<TTo>(object from);

        /// <summary>
        /// Maps a single source object to an object of type TTo that was already created.
        /// </summary>
        /// <typeparam name="TTo">The type of object being mapped to.</typeparam>
        /// <param name="to">The instance of the object being mapped to.</param>
        /// <param name="from">The object being mapped from.</param>
        /// <returns></returns>
        TTo Map<TTo>(TTo to, object from);


        #endregion

    }
}
