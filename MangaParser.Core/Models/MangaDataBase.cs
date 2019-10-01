using MangaParser.Core.Interfaces;
using System;

namespace MangaParser.Core.Models
{
    /// <summary>
    /// Provides an object representation of a manga parameter with its own URI.
    /// </summary>
    [Serializable]
    public class MangaDataBase : IData
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MangaDataBase"/> class with the specified parameters.
        /// </summary>
        /// <param name="value">Data value.</param>
        /// <param name="dataUri">Data URI.</param>
        public MangaDataBase(string value, string dataUri)
        {
            this.Value = value;

            if (Uri.IsWellFormedUriString(dataUri, UriKind.Absolute))
                this.Link = new Uri(dataUri);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MangaDataBase"/> class with the specified parameters.
        /// </summary>
        /// <param name="value">Data value.</param>
        /// <param name="dataUri">Data URI.</param>
        public MangaDataBase(string value, Uri dataUri)
        {
            this.Value = value;
            this.Link = dataUri;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the data URI.
        /// </summary>
        public Uri Link { get; }

        /// <summary>
        /// Gets the data value.
        /// </summary>
        public string Value { get; }

        #endregion Properties

        #region Methods

        public override string ToString()
        {
            return Value;
        }

        #endregion Methods
    }
}