using System;

namespace MangaParser.Core.Interfaces
{
    public interface IParser : IParserSync, IParserAsync
    {
        #region Properties

        Uri BaseUrl { get; }

        #endregion Properties
    }
}