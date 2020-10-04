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
        //TODO: add System.Threading.CancellationToken to methods
        #region Methods

        #region Search

        Task<IEnumerable<IMangaObject>> SearchMangaAsync(string query);

        #endregion Search

        #region GetManga

        Task<IMangaObject> GetMangaAsync(IMangaObject manga);

        Task<IMangaObject> GetMangaAsync(string url);

        Task<IMangaObject> GetMangaAsync(Uri uri);

        #endregion GetManga

        #region GetChapters

        Task<IEnumerable<IChapter>> GetChaptersAsync(IMangaObject manga);

        Task<IEnumerable<IChapter>> GetChaptersAsync(string url);

        Task<IEnumerable<IChapter>> GetChaptersAsync(Uri url);

        #endregion GetChapters

        #region GetPages

        Task<IEnumerable<IDataBase>> GetPagesAsync(IChapter chapter);

        Task<IEnumerable<IDataBase>> GetPagesAsync(string url);

        Task<IEnumerable<IDataBase>> GetPagesAsync(Uri url);

        #endregion GetPages

        #endregion Methods
    }
}