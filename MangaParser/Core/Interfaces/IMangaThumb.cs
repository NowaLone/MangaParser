using MangaParser.Core.Models;
using System;

namespace MangaParser.Core.Interfaces
{
    /// <summary>
    /// Defines a minimum information about a manga.
    /// </summary>
    public interface IMangaThumb
    {
        Name Name { get; }
        Uri MangaUri { get; }
    }
}