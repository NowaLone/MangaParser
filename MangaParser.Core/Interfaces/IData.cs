using System;

namespace MangaParser.Core.Interfaces
{
    /// <summary>
    /// Defines a minimum data parameters.
    /// </summary>
    public interface IData
    {
        #region Properties

        /// <summary>
        /// The data value.
        /// </summary>
        string Value { get; }

        /// <summary>
        /// The data URI.
        /// </summary>
        Uri Link { get; }

        #endregion Properties
    }
}