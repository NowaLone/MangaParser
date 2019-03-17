using System;

namespace MangaParser.Core.Interfaces
{
    /// <summary>
    /// Defines a minimum data parameters.
    /// </summary>
    public interface IData
    {
        /// <summary>
        /// Gets the data value.
        /// </summary>
        string Value { get; }

        /// <summary>
        /// Gets the data URI.
        /// </summary>
        Uri Link { get; }
    }
}