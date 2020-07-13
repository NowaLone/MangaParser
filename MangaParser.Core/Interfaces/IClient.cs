using System;

namespace MangaParser.Core.Interfaces
{
    public interface IClient : IParserSync, IParserAsync
    {
        #region Methods

        #region AddParser

        public void AddParser(IParser parser);

        #endregion AddParser

        #region RemoveParser

        public bool RemoveParser(string url);

        public bool RemoveParser(Uri url);

        public bool RemoveParser(IParser parser);

        public bool RemoveParser<T>() where T : IParser;

        #endregion RemoveParser

        #region GetParser

        public IParser GetParser(string url);

        public IParser GetParser(Uri url);

        public IParser GetParser<T>() where T : IParser;

        #endregion GetParser

        #endregion Methods
    }
}