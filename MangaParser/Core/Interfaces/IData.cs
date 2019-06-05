using System;

namespace MangaParser.Core.Interfaces
{
    /// <summary>
    /// Defines a minimum data parameters.
    /// </summary>
    public interface IData
    {
        /// <summary>
        /// The data value.
        /// </summary>
        string Value { get; }

        /// <summary>
        /// The data URI.
        /// </summary>
        Uri Link { get; }
    }
}