using MangaParser.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace MangaParser.Core.Models
{
    /// <summary>
    /// Provides an object representation of a manga parameter with its own url.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="Value"/>.</typeparam>
    public class DataBase<T> : IDataBase<T>, IEquatable<DataBase<T>>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DataBase{T}"/> class with the specified parameters.
        /// </summary>
        /// <param name="value">Data value.</param>
        /// <param name="url">Data url.</param>
        public DataBase(T value, Uri url)
        {
            Value = value;
            Url = url;
        }

        /// <inheritdoc cref="DataBase{T}(T, Uri)"/>
        public DataBase(T value, string url)
        {
            Value = value;

            if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                Url = new Uri(url);
            }
        }

        #endregion Constructors

        #region Properties

        public virtual Uri Url { get; }

        public virtual T Value { get; }

        #endregion Properties

        #region Methods

        #region Default overrides

        public override bool Equals(object obj)
        {
            return Equals(obj as DataBase<T>);
        }

        public bool Equals(DataBase<T> other)
        {
            return other != null &&
                   EqualityComparer<Uri>.Default.Equals(Url, other.Url) &&
                   EqualityComparer<T>.Default.Equals(Value, other.Value);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Url, Value);
        }

        /// <summary>
        /// Returns a string representation of <see cref="Value"/>.
        /// </summary>
        /// <returns>A string representation of <see cref="Value"/>.</returns>
        public override string ToString()
        {
            return Value?.ToString();
        }

        #endregion Default overrides

        #endregion Methods
    }

    /// <inheritdoc cref="DataBase{T}"/>
    public class DataBase : DataBase<object>, IDataBase
    {
        #region Constructors

        /// <inheritdoc cref="DataBase(object, Uri)"/>
        public DataBase(object value, string url) : base(value, url)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataBase"/> class with the specified parameters.
        /// </summary>
        /// <inheritdoc cref="DataBase{T}(T, Uri)"/>
        public DataBase(object value, Uri url) : base(value, url)
        {
        }

        #endregion Constructors
    }
}