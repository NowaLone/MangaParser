using System;
using System.Collections.Generic;

namespace MangaParser.Core.Interfaces
{
    public interface IClient : IParserSync, IParserAsync
    {
        #region Methods

        #region AddParser

        void AddParser(IParser parser);

        #endregion AddParser

        #region RemoveParser

        bool RemoveParser(string url);

        bool RemoveParser(Uri url);

        bool RemoveParser(IParser parser);

        bool RemoveParser<T>() where T : IParser;

        #endregion RemoveParser

        #region GetParser

        IParser GetParser(string url);

        IParser GetParser(Uri url);

        IParser GetParser<T>() where T : IParser;

        /// <summary>
        /// Returns a collection of <see cref="IParser"/> where <see cref="IParser.BaseUrl"/> host contains <paramref name="name"/> value.
        /// </summary>
        /// <param name="name">A part of <see cref="Uri.Host"/> for searching.</param>
        /// <returns>A collection of <see cref="IParser"/> where <see cref="IParser.BaseUrl"/> host contains <paramref name="name"/> value.</returns>
        IEnumerable<IParser> GetParsers(string name);

        #endregion GetParser

        #endregion Methods
    }
}