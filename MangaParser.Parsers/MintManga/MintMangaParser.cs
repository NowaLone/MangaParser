using MangaParser.Parsers.ReadManga;

namespace MangaParser.Parsers.MintManga
{
    public class MintMangaParser : ReadMangaParser
    {
        #region Constructors

        public MintMangaParser(string baseUri = "http://mintmanga.live") : base(baseUri)
        {
        }

        #endregion Constructors
    }
}