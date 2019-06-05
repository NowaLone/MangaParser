using MangaParser.Core.Models;
using System;

namespace MangaParser.Core.Interfaces
{
    /// <summary>
    /// Defines a minimum information about a manga.
    /// </summary>
    public interface IMangaThumb
    {
        /// <summary>
        /// The manga title.
        /// </summary>
        MangaName Name { get; }

        /// <summary>
        /// A link to the manga page.
        /// </summary>
        Uri MangaUri { get; }
    }
}