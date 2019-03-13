namespace MangaParser.Interfaces
{
    /// <summary>
    /// Defines default manga parameter.
    /// </summary>
    public interface IData
    {
        /// <summary>
        /// Gets the data value.
        /// </summary>
        string Value { get; }
        /// <summary>
        /// Gets the data link.
        /// </summary>
        string Link { get; }
    }
}