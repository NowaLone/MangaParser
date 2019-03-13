using HtmlAgilityPack;

namespace MangaParser.Parsers
{
    public class MintMangaParser : ReadMangaParser
    {
        public MintMangaParser(HtmlWeb web) : base(web)
        {
            BaseUrl = "mintmanga.com";
        }
    }
}