using HtmlAgilityPack;
using MangaParser.Core.Interfaces;
using MangaParser.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MangaParser.Parsers.HtmlWebParsers.Mangatown
{
    public class MangatownParser : CoreParser
    {
        #region Constructors

        public MangatownParser(string baseUri = "https://www.mangatown.com") : base(baseUri)
        {
        }

        #endregion Constructors

        #region Methods

        #region Search

        public override IEnumerable<IMangaBase> SearchManga(string query)
        {
            query = query is null ? String.Empty : query.Replace(' ', '+');

            var htmlDoc = Web.Load(BaseUrl + $"/search.php?name={query}");

            return SearchMangaCore(htmlDoc);
        }

        protected override IEnumerable<IMangaBase> SearchMangaCore(HtmlDocument htmlDoc)
        {
            if (htmlDoc is null)
            {
                throw new ArgumentNullException(nameof(htmlDoc));
            }

            var mainNode = htmlDoc.DocumentNode.SelectSingleNode("/html/body/section/article/div/div[2]/ul");

            return GetSearchResult(mainNode);
        }

        #endregion Search

        #region GetManga

        public override IManga GetManga(Uri url)
        {
            if (url is null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            var htmlDoc = Web.Load(url);

            var manga = GetMangaCore(htmlDoc);

            (manga as MangaObject).Url = url;

            return manga;
        }

        public override async Task<IManga> GetMangaAsync(Uri url)
        {
            if (url is null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            var htmlDoc = await Web.LoadFromWebAsync(url.OriginalString).ConfigureAwait(false);

            var manga = GetMangaCore(htmlDoc);

            (manga as MangaObject).Url = url;

            return manga;
        }

        protected override IManga GetMangaCore(HtmlDocument htmlDoc)
        {
            if (htmlDoc is null)
            {
                throw new ArgumentNullException(nameof(htmlDoc));
            }

            var mainNode = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='article_content']");

            return GetMangaData(mainNode);
        }

        #endregion GetManga

        #region GetChapters

        protected override IEnumerable<IChapter> GetChaptersCore(HtmlDocument htmlDoc)
        {
            if (htmlDoc is null)
            {
                throw new ArgumentNullException(nameof(htmlDoc));
            }

            var mainNode = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='chapter_content']");

            return GetChapters(mainNode);
        }

        #endregion GetChapters

        #region GetPages

        protected override IEnumerable<IPage> GetPagesCore(HtmlDocument htmlDoc)
        {
            if (htmlDoc is null)
            {
                throw new ArgumentNullException(nameof(htmlDoc));
            }

            var mainNode = htmlDoc.DocumentNode.SelectSingleNode("/html/body/section");

            return GetMangaPages(mainNode);
        }

        #endregion GetPages

        #region Private Methods

        private MangaObject GetMangaData(HtmlNode mainNode)
        {
            var infoNode = mainNode?.SelectSingleNode(".//div[@class='detail_info clearfix']/ul");

            var manga = new MangaObject
            {
                Name = GetName(mainNode),
                Description = GetDescription(mainNode),
                Covers = new Cover[] { new Cover(null, null, new Uri(mainNode?.SelectSingleNode(".//img").Attributes["src"].Value)) },
                Genres = GetInfoData(infoNode, 4).Concat(GetInfoData(infoNode, 5)).ToArray(),
                Autors = GetInfoData(infoNode, 6),
                Illustrators = GetInfoData(infoNode, 7),
            };

            return manga;
        }

        private async Task<MangaPage[]> GetMangaPagesAsync(HtmlNode mainNode)
        {
            var Counters = mainNode.SelectSingleNode(".//div[@class='page_select']").SelectNodes(".//select/option[not(contains(@value,'featured'))]");

            MangaPage[] pages;

            if (Counters != null)
            {
                pages = new MangaPage[Counters.Count];

                pages[0] = new MangaPage(mainNode.SelectSingleNode(".//img[@id='image']")?.Attributes["src"]?.Value);

                HtmlDocument htmlDoc;

                for (int i = 1; i < Counters.Count; i++)
                {
                    htmlDoc = await Web.LoadFromWebAsync(new Uri(BaseUrl, Counters[i].Attributes["value"]?.Value).OriginalString);

                    pages[i] = new MangaPage(htmlDoc.DocumentNode.SelectSingleNode(".//img[@id='image']")?.Attributes["src"]?.Value);
                }
            }
            else
                pages = new MangaPage[0];

            return pages;
        }

        private IEnumerable<MangaPage> GetMangaPages(HtmlNode mainNode)
        {
            var Counters = mainNode.SelectSingleNode(".//div[@class='page_select']").SelectNodes(".//select/option[not(contains(@value,'featured'))]");

            if (Counters != null)
            {
                yield return new MangaPage(mainNode.SelectSingleNode(".//img[@id='image']")?.Attributes["src"]?.Value);

                HtmlDocument htmlDoc;

                for (int i = 1; i < Counters.Count; i++)
                {
                    htmlDoc = Web.Load(new Uri(BaseUrl, Counters[i].Attributes["value"]?.Value));

                    yield return new MangaPage(htmlDoc.DocumentNode.SelectSingleNode(".//img[@id='image']")?.Attributes["src"]?.Value);
                }
            }
        }

        #region Search methods

        private IEnumerable<MangaObject> GetSearchResult(HtmlNode mainNode)
        {
            var thumbs = mainNode?.SelectNodes("./li");

            if (thumbs != null)
            {
                for (int i = 0; i < thumbs.Count; i++)
                {
                    var manga = new MangaObject
                    {
                        Autors = new DataBase[] { new DataBase(Decode(thumbs[i].SelectSingleNode("./p[4]/a")?.InnerText), thumbs[i].SelectSingleNode("./p[4]/a")?.Attributes["href"]?.Value) },
                        Genres = GetThumbGenres(thumbs[i].SelectSingleNode("./p[3]")),
                        Covers = new Cover[] { new Cover(thumbs[i].SelectSingleNode("./a/img")?.Attributes["src"]?.Value, null, null) },
                        Url = new Uri(BaseUrl, thumbs[i].SelectSingleNode("./a")?.Attributes["href"]?.Value),
                        Name = new Name(null, null, thumbs[i].SelectSingleNode("./a")?.Attributes["title"]?.Value),
                    };

                    yield return manga;
                }
            }
        }

        private DataBase[] GetThumbGenres(HtmlNode node)
        {
            var genres = node?.SelectNodes("./a");

            DataBase[] Genres;

            if (genres != null)
            {
                Genres = new DataBase[genres.Count];

                for (int i = 0; i < genres.Count; i++)
                {
                    Genres[i] = new DataBase(Decode(genres[i].InnerText), new Uri(BaseUrl, genres[i].Attributes["href"].Value));
                }
            }
            else
            {
                Genres = new DataBase[0];
            }

            return Genres;
        }

        #endregion Search methods

        #region Data Getting Methods

        private Chapter[] GetChapters(HtmlNode mainNode)
        {
            var chapters = mainNode?.SelectNodes("./ul/li");

            Chapter[] Chapters;

            if (chapters != null)
            {
                Chapters = new Chapter[chapters.Count];

                for (int i = chapters.Count - 1; i >= 0; i--)
                {
                    DateTime.TryParse(Decode(chapters[i].SelectSingleNode("./span[@class='time']")?.InnerText), System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime date);

                    string name = Decode(chapters[i].SelectSingleNode("./a")?.InnerText);
                    string additional = String.Empty;

                    var additionals = chapters[i].SelectNodes("./span[not(@class='time')]");

                    if (additionals != null)
                    {
                        for (int j = 0; j < additionals.Count; j++)
                        {
                            additional += " " + Decode(additionals[j].InnerText);
                        }
                    }

                    Chapters[chapters.Count - 1 - i] = new Chapter(name + additional, new Uri(BaseUrl, chapters[i].SelectSingleNode("./a")?.Attributes["href"]?.Value), date);
                }
            }
            else
            {
                Chapters = new Chapter[0];
            }

            return Chapters;
        }

        private DataBase[] GetInfoData(HtmlNode infoNode, int liNumber)
        {
            var data = infoNode?.SelectNodes($"./li[{liNumber}]/a");

            DataBase[] Data;

            if (data != null)
            {
                Data = new DataBase[data.Count];

                for (int i = 0; i < data.Count; i++)
                {
                    Data[i] = new DataBase(Decode(data[i]?.InnerText), new Uri(BaseUrl, data[i].Attributes["href"].Value));
                }
            }
            else
            {
                Data = new DataBase[0];
            }

            return Data;
        }

        private string GetDescription(HtmlNode mainNode)
        {
            var descNode = mainNode?.SelectSingleNode(".//span[@id='show']");

            if (descNode != null)
            {
                return Decode(descNode.InnerText);
            }
            else
                return null;
        }

        private Name GetName(HtmlNode mainNode)
        {
            var orName = Decode(mainNode?.SelectSingleNode(".//div[@class='detail_info clearfix']/ul/li[3]/text()")?.InnerText).Split(';')[0];
            var engName = Decode(mainNode?.SelectSingleNode("./h1")?.InnerText);

            if (orName != null || engName != null)
            {
                return new Name(null, engName == String.Empty ? null : engName, orName == String.Empty ? null : orName);
            }
            else
                return new Name();
        }

        #endregion Data Getting Methods

        #endregion Private Methods

        #endregion Methods
    }
}