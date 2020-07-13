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

        IEnumerable<IMangaThumb> SearchManga(string query);

        #endregion Search

        #region GetManga

        IManga GetManga(IMangaThumb manga);

        IManga GetManga(string url);

        IManga GetManga(Uri url);

        #endregion GetManga

        #region GetChapters

        IEnumerable<IChapter> GetChapters(IMangaThumb manga);

        IEnumerable<IChapter> GetChapters(string url);

        IEnumerable<IChapter> GetChapters(Uri url);

        #endregion GetChapters

        #region GetPages

        IEnumerable<IPage> GetPages(IChapter chapter);

        IEnumerable<IPage> GetPages(string url);

        IEnumerable<IPage> GetPages(Uri url);

        #endregion GetPages

        #endregion Methods
    }
}