using System;
using System.Collections.Generic;

namespace MangaParser.Core.Interfaces
{
    /// <summary>
    ///  Defines methods to manipulate a parser.
    /// </summary>
    public interface IParser
    {
        Uri BaseUri { get; }

        IEnumerable<IMangaThumb> SearchManga(string query);

        IManga GetManga(IMangaThumb mangaThumb);
        IManga GetManga(string mangaUri);
        IManga GetManga(Uri mangaUri);

        IEnumerable<IChapter> GetChapters(IMangaThumb manga);
        IEnumerable<IChapter> GetChapters(string mangaUri);
        IEnumerable<IChapter> GetChapters(Uri mangaUri);

        IEnumerable<IPage> GetPages(IChapter chapter);
        IEnumerable<IPage> GetPages(string chapterUri);
        IEnumerable<IPage> GetPages(Uri chapterUri);
    }
}