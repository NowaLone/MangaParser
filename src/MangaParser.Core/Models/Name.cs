using MangaParser.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace MangaParser.Core.Models
{
    /// <summary>
    ///  Provides an object representation of the name of a manga, chapter, etc.
    /// </summary>
    [Serializable]
    public class Name : IName, IEquatable<Name>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Name"/> class with the <see langword="null"/> parameters.
        /// </summary>
        public Name()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Name"/> class with the specified parameters.
        /// </summary>
        /// <param name="localized">A data with a string that represent the localized name.</param>
        /// <param name="english">A data with a string that represent the english name.</param>
        /// <param name="original">A data with a string that represent the original name.</param>
        public Name(IDataBase<string> localized = default, IDataBase<string> english = default, IDataBase<string> original = default)
        {
            English = english;
            Localized = localized;
            Original = original;
        }

        /// <param name="localized">A string that represent the localized name.</param>
        /// <param name="english">A string that represent the english name.</param>
        /// <param name="original">A string that represent the original name.</param>
        /// <param name="url">An url for <see cref="IDataBase{T}.Url"/>.</param>
        /// <inheritdoc cref="Name(IDataBase{string}, IDataBase{string}, IDataBase{string})"/>
        public Name(string localized = default, string english = default, string original = default, Uri url = default)
        {
            Localized = localized != default ? new DataBase<string>(localized, url) : null;
            English = english != default ? new DataBase<string>(english, url) : null;
            Original = original != default ? new DataBase<string>(original, url) : null;
        }

        /// <inheritdoc cref="Name(string, string, string, Uri)"/>
        public Name(string localized = default, string english = default, string original = default, string url = default) : this(localized, english, original, Uri.TryCreate(url, UriKind.Absolute, out var result) ? result : null)
        {
        }

        #endregion Constructors

        #region Properties

        public virtual IDataBase<string> Localized { get; }
        public virtual IDataBase<string> English { get; }
        public virtual IDataBase<string> Original { get; }

        #endregion Properties

        #region Methods

        #region Default overrides

        public override bool Equals(object obj)
        {
            return Equals(obj as Name);
        }

        public bool Equals(Name other)
        {
            return other != null &&
                   EqualityComparer<IDataBase<string>>.Default.Equals(Localized, other.Localized) &&
                   EqualityComparer<IDataBase<string>>.Default.Equals(English, other.English) &&
                   EqualityComparer<IDataBase<string>>.Default.Equals(Original, other.Original);
        }

        public override int GetHashCode()
        {
            int hashCode = -1585762065;
            hashCode = hashCode * -1521134295 + EqualityComparer<IDataBase<string>>.Default.GetHashCode(Localized);
            hashCode = hashCode * -1521134295 + EqualityComparer<IDataBase<string>>.Default.GetHashCode(English);
            hashCode = hashCode * -1521134295 + EqualityComparer<IDataBase<string>>.Default.GetHashCode(Original);
            return hashCode;
        }

        /// <summary>
        /// Returns a first non-empty or whitespace name.
        /// </summary>
        /// <returns>A first non-empty or whitespace name.</returns>
        public override string ToString()
        {
            return !String.IsNullOrWhiteSpace(Localized?.Value)
                ? Localized.Value
                : !String.IsNullOrWhiteSpace(English?.Value)
                ? English.Value
                : Original?.Value;
        }

        #endregion Default overrides

        #endregion Methods
    }
}