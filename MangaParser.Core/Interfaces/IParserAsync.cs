using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MangaParser.Core.Interfaces
{
    /// <summary>
    ///  Defines asynchronous methods to manipulate a parser.
    /// </summary>
    public interface IParserAsync
    {
        #region Methods

        #region Search

        Task<IEnumerable<IMangaThumb>> SearchMangaAsync(string query);

        #endregion Search

        #region GetManga

        Task<IManga> GetMangaAsync(IMangaThumb manga);

        Task<IManga> GetMangaAsync(string url);

        Task<IManga> GetMangaAsync(Uri uri);

        #endregion GetManga

        #region GetChapters

        Task<IEnumerable<IChapter>> GetChaptersAsync(IMangaThumb manga);

        Task<IEnumerable<IChapter>> GetChaptersAsync(string url);

        Task<IEnumerable<IChapter>> GetChaptersAsync(Uri url);

        #endregion GetChapters

        #region GetPages

        Task<IEnumerable<IPage>> GetPagesAsync(IChapter chapter);

        Task<IEnumerable<IPage>> GetPagesAsync(string url);

        Task<IEnumerable<IPage>> GetPagesAsync(Uri url);

        #endregion GetPages

        #endregion Methods
    }
}