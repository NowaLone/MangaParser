namespace MangaParser.Parsers.Mangapanda
{
    public class MangapandaParser : Mangareader.MangareaderParser
    {
        #region Constructors

        public MangapandaParser(string baseUri = "http://www.mangapanda.com") : base(baseUri)
        {
        }

        #endregion Constructors
    }
}