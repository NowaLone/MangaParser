using MangaParser.Core.Interfaces;
using System;

namespace MangaParser.Core.Models
{
    /// <summary>
    /// Provides an object representation of a manga cover.
    /// </summary>
    [Serializable]
    public class MangaCover : IMangaCover
    {
        #region Constructors

        /// <summary>
        ///  Initializes a new instance of the <see cref="MangaCover"/> class with the specified URIs.
        /// </summary>
        /// <param name="smallSize">A low resolution cover URI.</param>
        /// <param name="mediumSize">A mid-res cover URI.</param>
        /// <param name="largeSize">A hi-res cover URI.</param>
        public MangaCover(Uri smallSize = default(Uri), Uri mediumSize = default(Uri), Uri largeSize = default(Uri))
        {
            Large = largeSize;
            Medium = mediumSize;
            Small = smallSize;
        }

        /// <summary>
        ///  Initializes a new instance of the <see cref="MangaCover"/> class with the specified URIs.
        /// </summary>
        /// <param name="smallSize">A low resolution cover URI.</param>
        /// <param name="mediumSize">A medium resolution cover URI.</param>
        /// <param name="largeSize">A high resolution cover URI.</param>
        public MangaCover(string smallSize = default(string), string mediumSize = default(string), string largeSize = default(string))
        {
            Large = Uri.IsWellFormedUriString(largeSize, UriKind.Absolute) ? new Uri(largeSize) : null;
            Medium = Uri.IsWellFormedUriString(mediumSize, UriKind.Absolute) ? new Uri(mediumSize) : null;
            Small = Uri.IsWellFormedUriString(smallSize, UriKind.Absolute) ? new Uri(smallSize) : null;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets a high resolution cover URI.
        /// </summary>
        public Uri Large { get; }

        /// <summary>
        ///  Gets a medium resolution cover URI.
        /// </summary>
        public Uri Medium { get; }

        /// <summary>
        /// Gets a low resolution cover URI.
        /// </summary>
        public Uri Small { get; }

        #endregion Properties

        #region Methods

        public override string ToString()
        {
            if (Large != null)
                return Large.OriginalString;
            else if (Medium != null)
                return Medium.OriginalString;
            else if (Small != null)
                return Small.OriginalString;
            return
                null;
        }

        #endregion Methods
    }
}