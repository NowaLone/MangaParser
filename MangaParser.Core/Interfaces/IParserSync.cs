using System;
using System.Collections.Generic;

namespace MangaParser.Core.Interfaces
{
    /// <summary>
    ///  Defines synchronous methods to manipulate a parser.
    /// </summary>
    public interface IParserSync
    {
        #region Methods

        #region Search

        IEnumerable<IMangaObject> SearchManga(string query);

        #endregion Search

        #region GetManga

        IMangaObject GetManga(IMangaObject manga);

        IMangaObject GetManga(string url);

        IMangaObject GetManga(Uri url);

        #endregion GetManga

        #region GetChapters

        IEnumerable<IChapter> GetChapters(IMangaObject manga);

        IEnumerable<IChapter> GetChapters(string url);

        IEnumerable<IChapter> GetChapters(Uri url);

        #endregion GetChapters

        #region GetPages

        IEnumerable<IDataBase> GetPages(IChapter chapter);

        IEnumerable<IDataBase> GetPages(string url);

        IEnumerable<IDataBase> GetPages(Uri url);

        #endregion GetPages

        #endregion Methods
    }
}