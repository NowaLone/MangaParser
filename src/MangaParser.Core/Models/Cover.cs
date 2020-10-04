using MangaParser.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace MangaParser.Core.Models
{
    /// <summary>
    /// Provides an object representation of a cover of a manga, chapter, etc.
    /// </summary>
    [Serializable]
    public class Cover : ICover, IEquatable<Cover>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Cover"/> class with the <see langword="null"/> urls.
        /// </summary>
        public Cover()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Cover"/> class with the specified urls.
        /// </summary>
        /// <param name="large">A data with a hi-res cover url.</param>
        /// <param name="medium">A data with a mid-res cover url.</param>
        /// <param name="small">A data with a low-res cover url.</param>
        public Cover(IDataBase large = default, IDataBase medium = default, IDataBase small = default)
        {
            Large = large;
            Medium = medium;
            Small = small;
        }

        /// <param name="large">A hi-res cover url.</param>
        /// <param name="medium">A mid-res cover url.</param>
        /// <param name="small">A low-res cover url.</param>
        /// <inheritdoc cref="Cover(IDataBase, IDataBase, IDataBase)"/>
        public Cover(Uri large = default, Uri medium = default, Uri small = default)
        {
            Large = large != default ? new DataBase(default, large) : null;
            Medium = medium != default ? new DataBase(default, medium) : null;
            Small = small != default ? new DataBase(default, small) : null;
        }

        /// <param name="large">A hi-res cover url.</param>
        /// <param name="medium">A mid-res cover url.</param>
        /// <param name="small">A low-res cover url.</param>
        /// <inheritdoc cref="Cover(IDataBase, IDataBase, IDataBase)"/>
        public Cover(string large = default, string medium = default, string small = default)
        {
            if (Uri.TryCreate(large, UriKind.Absolute, out Uri result))
            {
                Large = new DataBase(default, result);
            }

            if (Uri.TryCreate(medium, UriKind.Absolute, out result))
            {
                Medium = new DataBase(default, result);
            }

            if (Uri.TryCreate(small, UriKind.Absolute, out result))
            {
                Small = new DataBase(default, result);
            }
        }

        #endregion Constructors

        #region Properties

        public virtual IDataBase Large { get; }

        public virtual IDataBase Medium { get; }

        public virtual IDataBase Small { get; }

        #endregion Properties

        #region Methods

        #region Default overrides

        public override bool Equals(object obj)
        {
            return Equals(obj as Cover);
        }

        public bool Equals(Cover other)
        {
            return other != null &&
                   EqualityComparer<IDataBase>.Default.Equals(Large, other.Large) &&
                   EqualityComparer<IDataBase>.Default.Equals(Medium, other.Medium) &&
                   EqualityComparer<IDataBase>.Default.Equals(Small, other.Small);
        }

        public override int GetHashCode()
        {
            int hashCode = -1928674874;
            hashCode = hashCode * -1521134295 + EqualityComparer<IDataBase>.Default.GetHashCode(Large);
            hashCode = hashCode * -1521134295 + EqualityComparer<IDataBase>.Default.GetHashCode(Medium);
            hashCode = hashCode * -1521134295 + EqualityComparer<IDataBase>.Default.GetHashCode(Small);
            return hashCode;
        }

        /// <summary>
        /// Returns a first non-null cover url.
        /// </summary>
        /// <returns>A first non-null cover url.</returns>
        public override string ToString()
        {
            return Large?.Url != null
                ? Large.Url.AbsoluteUri
                : Medium?.Url != null
                ? Medium.Url.AbsoluteUri
                : Small?.Url != null
                ? Small.Url.AbsoluteUri
                : default;
        }

        #endregion Default overrides

        #endregion Methods
    }
}