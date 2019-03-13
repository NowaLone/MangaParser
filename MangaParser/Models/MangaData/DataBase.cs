using MangaParser.Interfaces;

namespace MangaParser.Models.MangaData
{
    /// <summary>
    /// Provides an object representation of a manga parameter with its own link.
    /// </summary>
    public class DataBase : IData
    {
        /// <summary>
        /// Gets the data value.
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// Gets the data link.
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataBase"/> class with the empty parameters.
        /// </summary>
        public DataBase() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="MangaObject"/> class with the specified parameters.
        /// </summary>
        /// <param name="value">Data value.</param>
        /// <param name="link">Data link.</param>
        public DataBase(string value, string link)
        {
            Value = value;
            Link = link;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}