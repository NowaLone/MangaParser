using MangaParser.Core.Interfaces;
using System;

namespace MangaParser.Core.Models
{
    /// <summary>
    /// Provides an object representation of a manga chapter.
    /// </summary>
    public class Chapter : DataBase<IName>, IChapter
    {
        #region Constructors

        /// <inheritdoc cref="Chapter(IName, Uri, DateTime, ICover)"/>
        public Chapter(IName value, string url, DateTime addedDate, ICover cover = default) : base(value, url)
        {
            AddedDate = addedDate;
            Cover = cover;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Chapter"/> class with the specified parameters.
        /// </summary>
        /// <param name="value">A data with a chapter name.</param>
        /// <param name="url">An url to the chapter.</param>
        /// <param name="addedDate">A chapter added date.</param>
        /// <param name="cover">A data with a chapter cover.</param>
        public Chapter(IName value, Uri url, DateTime addedDate, ICover cover = default) : base(value, url)
        {
            AddedDate = addedDate;
            Cover = cover;
        }

        #endregion Constructors

        #region Properties

        public DateTime AddedDate { get; }
        public ICover Cover { get; }

        #endregion Properties
    }
}