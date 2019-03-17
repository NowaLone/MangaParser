using MangaParser.Core.Models;

namespace MangaParser.Core.Interfaces
{
    /// <summary>
    /// Defines a minimum manga parameters.
    /// </summary>
    public interface IManga : IMangaThumb
    {
        MangaCover[] Covers { get; }
        IData[] Autors { get; }
        IData[] Genres { get; }
        string Description { get; }
    }
}