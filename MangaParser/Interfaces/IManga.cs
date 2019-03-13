using MangaParser.Models.MangaData;
using System.Collections.Generic;

namespace MangaParser.Interfaces
{
    /// <summary>
    /// Defines default manga parameters.
    /// </summary>
    public interface IManga
    {
        string EnglishTitle { get; }
        string LocalizedTitle { get; }
        List<DataBase> Autors { get; }
        List<DataBase> Genres { get; }
        List<Cover> Covers { get; }
    }
}