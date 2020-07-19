namespace MangaParser.Core.Interfaces
{
    /// <summary>
    /// Defines a minimum information about the name of a manga, chapter, etc.
    /// </summary>
    public interface IName
    {
        #region Properties

        /// <summary>
        /// Gets the data with a localized name.
        /// </summary>
        /// <returns>The data with a localized name.</returns>
        IDataBase<string> Localized { get; }

        /// <summary>
        /// Gets the data with an english name.
        /// </summary>
        /// <returns>The data with an english name.</returns>
        IDataBase<string> English { get; }

        /// <summary>
        /// Gets the data with an original (Japanese) name.
        /// </summary>
        /// <returns>The data with an original (Japanese) name.</returns>
        IDataBase<string> Original { get; }

        #endregion Properties
    }
}