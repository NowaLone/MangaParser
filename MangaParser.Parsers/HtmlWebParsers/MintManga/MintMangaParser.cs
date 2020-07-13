using MangaParser.Parsers.HtmlWebParsers.ReadManga;

namespace MangaParser.Parsers.HtmlWebParsers.MintManga
{
    public class MintMangaParser : ReadMangaParser
    {
        #region Constructors

        public MintMangaParser(string baseUrl = "https://mintmanga.live") : base(baseUrl)
        {
        }

        #endregion Constructors
    }
}