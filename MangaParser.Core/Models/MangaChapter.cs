using MangaParser.Core.Interfaces;
using System;

namespace MangaParser.Core.Models
{
    /// <summary>
    ///  Provides an object representation of a manga chapter.
    /// </summary>
    public class MangaChapter : MangaDataBase, IChapter
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MangaChapter"/> class with the specified parameters.
        /// </summary>
        /// <param name="chapterName">Chapter name.</param>
        /// <param name="chapterUri">Chapter URI.</param>
        /// <param name="date">Chapter added date.</param>
        public MangaChapter(string chapterName, string chapterUri, DateTime date) : base(chapterName, chapterUri)
        {
            AddedDate = date;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MangaChapter"/> class with the specified parameters.
        /// </summary>
        /// <param name="chapterName">Chapter name.</param>
        /// <param name="chapterUri">Chapter URI.</param>
        /// <param name="date">Chapter added date.</param>
        public MangaChapter(string chapterName, Uri chapterUri, DateTime date) : base(chapterName, chapterUri)
        {
            AddedDate = date;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the chapter added date.
        /// </summary>
        public DateTime AddedDate { get; }

        /// <summary>
        /// Gets the chapter name.
        /// </summary>
        public string Name => Value;

        /// <summary>
        /// Gets the chapter URI.
        /// </summary>
        public Uri ChapterUri => Link;

        #endregion Properties

        #region Methods

        public override string ToString()
        {
            return Name + "\t\tAdded date: " + AddedDate.ToShortDateString();
        }

        #endregion Methods
    }
}