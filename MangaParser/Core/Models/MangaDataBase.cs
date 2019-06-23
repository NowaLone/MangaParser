using MangaParser.Core.Interfaces;
using System;

namespace MangaParser.Core.Models
{
    /// <summary>
    /// Provides an object representation of a manga parameter with its own URI.
    /// </summary>
    public class MangaDataBase : IData
    {
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

        /// <summary>
        /// Gets the data URI.
        /// </summary>
        public Uri Link { get; }

        /// <summary>
        /// Gets the data value.
        /// </summary>
        public string Value { get; }

        public override string ToString()
        {
            return Value;
        }
    }
}