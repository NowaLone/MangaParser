namespace MangaParser.Core.Interfaces
{
    /// <summary>
    /// Defines a minimum information about the cover of a manga, chapter, etc.
    /// </summary>
    public interface ICover
    {
        #region Properties

        /// <summary>
        /// Gets the data with a high resolution cover url.
        /// </summary>
        /// <returns>The data with a high resolution cover url.</returns>
        IDataBase Large { get; }

        /// <summary>
        /// Gets the data with a medium resolution cover url.
        /// </summary>
        /// <returns>The data with a medium resolution cover url.</returns>
        IDataBase Medium { get; }

        /// <summary>
        /// Gets the data with a low resolution cover url.
        /// </summary>
        /// <returns>The data with a low resolution cover url.</returns>
        IDataBase Small { get; }

        #endregion Properties
    }
}