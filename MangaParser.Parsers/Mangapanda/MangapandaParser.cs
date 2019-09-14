namespace MangaParser.Parsers.Mangapanda
{
    public class MangapandaParser : Mangareader.MangareaderParser
    {
        #region Constructors

        public MangapandaParser(string baseUri = "https://www.mangapanda.com") : base(baseUri)
        {
        }

        #endregion Constructors
    }
}