using MangaParser.Parsers.ReadManga;

namespace MangaParser.Parsers.MintManga
{
    public class MintMangaParser : ReadMangaParser
    {
        #region Constructors

        public MintMangaParser(string baseUri = "http://mintmanga.com") : base(baseUri)
        {
        }

        #endregion Constructors
    }
}