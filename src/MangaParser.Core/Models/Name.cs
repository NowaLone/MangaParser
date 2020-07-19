using MangaParser.Core.Interfaces;
using System;

namespace MangaParser.Core.Models
{
    /// <summary>
    ///  Provides an object representation of the name of a manga, chapter, etc.
    /// </summary>
    public class Name : IName
    {
        #region Constructors

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

        #endregion Constructors

        #region Properties

        public IDataBase<string> Localized { get; }
        public IDataBase<string> English { get; }
        public IDataBase<string> Original { get; }

        #endregion Properties

        #region Methods

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

        #endregion Methods
    }
}