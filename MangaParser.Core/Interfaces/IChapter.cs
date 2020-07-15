using System;

namespace MangaParser.Core.Interfaces
{
    /// <summary>
    /// Defines a minimum information about the manga chapter.
    /// </summary>
    public interface IChapter : IDataBase<IName>
    {
        #region Properties

        /// <summary>
        /// Gets date when the chapter was added.
        /// </summary>
        /// <returns>Date when the chapter was added.</returns>
        DateTime AddedDate { get; }

        /// <summary>
        /// Gets the chapter cover.
        /// </summary>
        /// <returns>The chapter cover.</returns>
        ICover Cover { get; }

        #endregion Properties
    }
}