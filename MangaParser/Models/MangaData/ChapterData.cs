using System;

namespace MangaParser.Models.MangaData
{
    /// <summary>
    ///  Provides an object representation of a manga chapter.
    /// </summary>
    public class ChapterData : DataBase
    {
        /// <summary>
        /// Gets the chapter added date.
        /// </summary>
        public DateTime AddedDate { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChapterData"/> class with the empty parameters.
        /// </summary>
        public ChapterData() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ChapterData"/> class with the specified parameters.
        /// </summary>
        /// <param name="value">Chapter name.</param>
        /// <param name="link">Chapter link.</param>
        /// <param name="date">Chapter added date.</param>
        public ChapterData(string value, string link, DateTime date) : base(value, link)
        {
            AddedDate = date;
        }

        public override string ToString()
        {
            return Value + "\t\tAdded date: " + AddedDate.ToShortDateString();
        }
    }
}