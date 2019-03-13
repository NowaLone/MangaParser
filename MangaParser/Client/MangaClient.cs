using HtmlAgilityPack;
using MangaParser.Models;
using MangaParser.Models.MangaData;
using MangaParser.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MangaParser.Client
{
    public class MangaClient
    {
        public ReadMangaParser ReadManga { get; }
        public MintMangaParser MintManga { get; }

        private HtmlWeb web;

        /// <summary>
        /// Pattern for js array scraping.
        /// </summary>
        private const string pattern = "\\[.*?'(http.*?)'\\,\\\"(.*?)\\\"";
        /// <summary>
        /// Js array name.
        /// </summary>
        private const string jsArrayVar = "rm_h.init";

        public MangaClient()
        {
            web = new HtmlWeb();

            ReadManga = new ReadMangaParser(web);
            MintManga = new MintMangaParser(web);
        }

        public MangaObject GetMangaByLink(string link)
        {
            if (link.Contains(ReadManga.BaseUrl))
                return ReadManga.GetManga(link);
            else if (link.Contains(MintManga.BaseUrl))
                return MintManga.GetManga(link);
            else return null;
        }

        public IEnumerable<MangaTile> SearchManga(string query)
        {
            foreach (MangaTile manga in ReadManga.SearchManga(query))
                yield return manga;

            foreach (MangaTile manga in MintManga.SearchManga(query))
                yield return manga;
        }

        public IEnumerable<Uri> GetChapterPages(ChapterData chapter)
        {
            var htmlDoc = web.Load(chapter.Link);

            var scriptText = htmlDoc.DocumentNode.Descendants()?.Where(n => n.Name == "script" && n.InnerText.Contains(jsArrayVar)).First().InnerText;

            int index = scriptText.IndexOf(jsArrayVar);

            if (index != -1)
            {
                string array = scriptText.Substring(index + jsArrayVar.Length);

                if (array.Length > 0)
                {
                    Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);

                    MatchCollection matchs = regex.Matches(array);

                    if (matchs.Count > 0)
                    {
                        for (int i = 0; i < matchs.Count; i++)
                        {
                            yield return new Uri(matchs[i].Groups[1].Value + matchs[i].Groups[2].Value);
                        }
                    }
                }
            }
        }
    }
}