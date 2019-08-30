using System;

namespace MangaParser.Core.Interfaces
{
    /// <summary>
    /// Defines a minimum information about a manga chapter.
    /// </summary>
    public interface IChapter
    {
        #region Properties

        /// <summary>
        /// Date when the chapter was added.
        /// </summary>
        DateTime AddedDate { get; }
        
        /// <summary>
        /// The chapter title.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// A link to the chapter page.
        /// </summary>
        Uri ChapterUri { get; }

        #endregion Properties
    }
}