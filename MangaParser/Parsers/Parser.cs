using HtmlAgilityPack;
using System;

namespace MangaParser.Parsers
{
    public abstract class Parser
    {
        public Parser(Uri baseUri)
        {
            BaseUri = baseUri;
        }

        public Parser(string baseUri)
        {
            if (Uri.IsWellFormedUriString(baseUri, UriKind.Absolute))
                BaseUri = new Uri(baseUri);
        }

        public static HtmlWeb Web { get; } = new HtmlWeb();
        public Uri BaseUri { get; }

        protected virtual string Decode(string htmlText)
        {
            if (!String.IsNullOrEmpty(htmlText))
            {
                var text = System.Net.WebUtility.HtmlDecode(htmlText).Trim();

                // Remove all whitespaces
                return String.Join(" ", text.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
            }
            else
                return String.Empty;
        }
    }
}