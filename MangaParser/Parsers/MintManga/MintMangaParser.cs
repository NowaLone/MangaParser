using MangaParser.Parsers.ReadManga;

namespace MangaParser.Parsers.MintManga
{
    public class MintMangaParser : ReadMangaParser
    {
        public MintMangaParser(string baseUri = "http://mintmanga.com") : base(baseUri)
        {
        }
    }
}