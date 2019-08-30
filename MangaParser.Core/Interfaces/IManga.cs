using MangaParser.Core.Models;

namespace MangaParser.Core.Interfaces
{
    /// <summary>
    /// Defines a minimum manga parameters.
    /// </summary>
    public interface IManga : IMangaThumb
    {
        #region Properties

        /// <summary>
        /// The manga covers collection.
        /// </summary>
        MangaCover[] Covers { get; }

        /// <summary>
        /// The manga autors collection.
        /// </summary>
        IData[] Autors { get; }

        /// <summary>
        /// The manga genres collection.
        /// </summary>
        IData[] Genres { get; }

        /// <summary>
        /// The manga description.
        /// </summary>
        string Description { get; }

        #endregion Properties
    }
}