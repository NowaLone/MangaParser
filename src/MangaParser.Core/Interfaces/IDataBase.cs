using System;

namespace MangaParser.Core.Interfaces
{
    /// <summary>
    /// Defines a minimum data parameters.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="Value"/>.</typeparam>
    public interface IDataBase<T>
    {
        #region Properties

        /// <summary>
        /// Gets the data value.
        /// </summary>
        /// /// <returns>The data value.</returns>
        T Value { get; }

        /// <summary>
        /// Gets the data url.
        /// </summary>
        /// <returns>The data url.</returns>
        Uri Url { get; }

        #endregion Properties
    }

    /// <inheritdoc cref="IDataBase{T}"/>
    public interface IDataBase : IDataBase<object>
    {
    }
}