using HtmlAgilityPack;
using MangaParser.Models;
using System.Collections.Generic;

namespace MangaParser.Parsers
{
    public abstract class Parser
    {
        public Parser(HtmlWeb web)
        {
            Web = web;
        }

        protected abstract HtmlWeb Web { get; set; }
        public abstract string BaseUrl { get; protected set; }

        public abstract MangaObject GetManga(string link);
        public abstract MangaObject GetManga(MangaTile manga);
        public abstract IEnumerable<MangaTile> SearchManga(string query);
    }
}