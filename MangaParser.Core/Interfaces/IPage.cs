using System;

namespace MangaParser.Core.Interfaces
{
    /// <summary>
    /// Defines a minimum information about a manga page.
    /// </summary>
    public interface IPage
    {
        #region Properties

        /// <summary>
        /// A manga page URI.
        /// </summary>
        Uri PageUri { get; }

        #endregion Properties
    }
}