
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MangaParser.Core.Interfaces
{
    /// <summary>
    ///  Defines methods to manipulate a parser.
    /// </summary>
    public interface IParser
    {
        #region Properties

        Uri BaseUri { get; }

        #endregion Properties

        #region Synchronous Methods

        IEnumerable<IChapter> GetChapters(IMangaThumb manga);
        IEnumerable<IChapter> GetChapters(string mangaUri);
        IEnumerable<IChapter> GetChapters(Uri mangaUri);

        IEnumerable<IMangaThumb> SearchManga(string query);

        IEnumerable<IPage> GetPages(IChapter chapter);
        IEnumerable<IPage> GetPages(string chapterUri);
        IEnumerable<IPage> GetPages(Uri chapterUri);

        IManga GetManga(IMangaThumb mangaThumb);
        IManga GetManga(string mangaUri);
        IManga GetManga(Uri mangaUri);

        #endregion Synchronous Methods

        #region Asynchronous Methods

        Task<IEnumerable<IChapter>> GetChaptersAsync(IMangaThumb manga);
        Task<IEnumerable<IChapter>> GetChaptersAsync(string mangaUri);
        Task<IEnumerable<IChapter>> GetChaptersAsync(Uri mangaUri);

        Task<IEnumerable<IMangaThumb>> SearchMangaAsync(string query);

        Task<IEnumerable<IPage>> GetPagesAsync(IChapter chapter);
        Task<IEnumerable<IPage>> GetPagesAsync(string chapterUri);
        Task<IEnumerable<IPage>> GetPagesAsync(Uri chapterUri);

        Task<IManga> GetMangaAsync(IMangaThumb mangaThumb);
        Task<IManga> GetMangaAsync(string mangaUri);
        Task<IManga> GetMangaAsync(Uri mangaUri);

        #endregion Asynchronous Methods
    }
}