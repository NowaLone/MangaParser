using HtmlAgilityPack;
using MangaParser.Core.Interfaces;
using MangaParser.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MangaParser.Parsers.HtmlWebParsers.Mangareader
{
    public class MangareaderParser : CoreParser
    {
        #region Constructors

        public MangareaderParser(string baseUri = "https://www.mangareader.net") : base(baseUri)
        {
        }

        #endregion Constructors

        #region Methods

        #region Search

        public override IEnumerable<IMangaBase> SearchManga(string query)
        {
            query = query is null ? String.Empty : query.Replace(' ', '+');

            var htmlDoc = Web.Load(BaseUrl + $"search/?w={query}");

            return SearchMangaCore(htmlDoc);
        }

        protected override IEnumerable<IMangaBase> SearchMangaCore(HtmlDocument htmlDoc)
        {
            if (htmlDoc is null)
            {
                throw new ArgumentNullException(nameof(htmlDoc));
            }

            var mainNode = htmlDoc.DocumentNode.SelectSingleNode("//div[@id='mangaresults']");

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

            var mainNode = htmlDoc.DocumentNode.SelectSingleNode("//div[@id='wrapper_body']");

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

            var mainNode = htmlDoc.DocumentNode.SelectSingleNode("//div[@id='wrapper_body']");

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

            var mainNode = htmlDoc.DocumentNode.SelectSingleNode("//body/div[@id='container']");

            return GetMangaPages(mainNode);
        }

        #endregion GetPages

        #region Private Methods

        private MangaObject GetMangaData(HtmlNode mainNode)
        {
            var infoNode = mainNode?.SelectSingleNode("//div[@id='mangaproperties']/table");

            var manga = new MangaObject
            {
                Name = GetName(infoNode),
                Description = GetDescription(mainNode),
                Covers = GetCovers(mainNode),
                Genres = GetGenres(infoNode),
                Autors = GetInfoData(infoNode, 5),
                Illustrators = GetInfoData(infoNode, 6),
                ReleaseDate = GetInfoData(infoNode, 3),
            };

            return manga;
        }

        private async Task<MangaPage[]> GetMangaPagesAsync(HtmlNode mainNode)
        {
            string imgNodePath = mainNode?.XPath + "//img[@id='img']";

            var Counters = mainNode.SelectNodes("./div[@id='topchapter']/div[@id='navi']/div[@id='selectpage']/select/option");

            MangaPage[] pages;

            if (Counters != null)
            {
                pages = new MangaPage[Counters.Count];

                pages[0] = new MangaPage(mainNode.SelectSingleNode("//img[@id='img']")?.Attributes["src"]?.Value);

                HtmlDocument htmlDoc;

                for (int i = 1; i < Counters.Count; i++)
                {
                    htmlDoc = await Web.LoadFromWebAsync(new Uri(BaseUrl, Counters[i].Attributes["value"]?.Value).OriginalString);

                    pages[i] = new MangaPage(htmlDoc.DocumentNode.SelectSingleNode(imgNodePath)?.Attributes["src"]?.Value);
                }
            }
            else
                pages = new MangaPage[0];

            return pages;
        }

        private IEnumerable<MangaPage> GetMangaPages(HtmlNode mainNode)
        {
            string imgNodePath = mainNode?.XPath + "//img[@id='img']";

            var Counters = mainNode.SelectNodes("./div[@id='topchapter']/div[@id='navi']/div[@id='selectpage']/select/option");

            if (Counters != null)
            {
                yield return new MangaPage(mainNode.SelectSingleNode("//img[@id='img']")?.Attributes["src"]?.Value);

                HtmlDocument htmlDoc;

                for (int i = 1; i < Counters.Count; i++)
                {
                    htmlDoc = Web.Load(new Uri(BaseUrl, Counters[i].Attributes["value"]?.Value));

                    yield return new MangaPage(htmlDoc.DocumentNode.SelectSingleNode(imgNodePath)?.Attributes["src"]?.Value);
                }
            }
        }

        #region Search methods

        private IEnumerable<MangaObject> GetSearchResult(HtmlNode mainNode)
        {
            var thumbs = mainNode?.SelectNodes("//div[@class='mangaresultinner']");

            if (thumbs != null)
            {
                for (int i = 0; i < thumbs.Count; i++)
                {
                    string coverstr = thumbs[i].SelectSingleNode("./div[@class='imgsearchresults']")?.Attributes["style"]?.Value;

                    var manga = new MangaObject
                    {
                        Genres = GetThumbGenres(thumbs[i].SelectSingleNode("./div[@class='result_info c5']/div[@class='manga_genre']")),
                        Covers = new Cover[] { new Cover(small: FixCoverProtocol(coverstr.Substring(coverstr.IndexOf("('") + 2, coverstr.IndexOf("')") - coverstr.IndexOf("('") - 2))) },
                        Url = new Uri(BaseUrl, thumbs[i].SelectSingleNode("./div[@class='result_info c5']/div[@class='manga_name']/div/h3/a")?.Attributes["href"]?.Value),
                        Name = new Name(null, null, thumbs[i].SelectSingleNode("./div[@class='result_info c5']/div[@class='manga_name']/div/h3/a")?.InnerText),
                    };

                    yield return manga;
                }
            }
        }

        private DataBase[] GetThumbGenres(HtmlNode node)
        {
            var genres = node?.InnerText.Replace(" ", String.Empty).Split(',');

            DataBase[] Genres;

            if (genres != null)
            {
                Genres = new DataBase[genres.Length];

                for (int i = 0; i < genres.Length; i++)
                {
                    Genres[i] = new DataBase(genres[i], default(Uri));
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
            var chapters = mainNode?.SelectNodes("//div[@id='chapterlist']/table/tr");

            Chapter[] Chapters;

            if (chapters != null)
            {
                Chapters = new Chapter[chapters.Count - 1];

                for (int i = 1; i < chapters.Count; i++)
                {
                    DateTime.TryParseExact(Decode(chapters[i].SelectSingleNode("./td[2]")?.InnerText), "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime date);

                    var name = Decode(chapters[i].SelectSingleNode("./td/a")?.InnerText);
                    var additional = Decode(chapters[i].SelectSingleNode("./td/text()[3]")?.InnerText);

                    Chapters[i - 1] = new Chapter(name + (additional == ":" ? String.Empty : additional), new Uri(BaseUrl, chapters[i].SelectSingleNode("./td/a")?.Attributes["href"]?.Value), date);
                }
            }
            else
            {
                Chapters = new Chapter[0];
            }

            return Chapters;
        }

        private DataBase[] GetInfoData(HtmlNode infoNode, int trNumber)
        {
            var data = infoNode?.SelectSingleNode($"./tr[{trNumber}]/td[2]");

            DataBase[] Data;

            if (data != null)
            {
                Data = new DataBase[1];

                Data[0] = new DataBase(Decode(data?.InnerText), default(Uri));
            }
            else
            {
                Data = new DataBase[0];
            }

            return Data;
        }

        private Cover[] GetCovers(HtmlNode mainNode)
        {
            var imageNode = mainNode?.SelectSingleNode("//img");

            Cover[] images;

            if (imageNode != null)
            {
                images = new Cover[] { new Cover(FixCoverProtocol(imageNode.Attributes["src"]?.Value), null, null) };
            }
            else
                images = new Cover[0];

            return images;
        }

        private string GetDescription(HtmlNode mainNode)
        {
            var descNode = mainNode?.SelectSingleNode("./div[@id='readmangasum']/p");

            if (descNode != null)
            {
                return Decode(descNode.InnerText);
            }
            else
                return null;
        }

        private DataBase[] GetGenres(HtmlNode infoNode)
        {
            var genreNode = infoNode?.SelectNodes("./tr[8]//a");

            DataBase[] genres;

            if (genreNode != null)
            {
                genres = new DataBase[genreNode.Count];

                for (int i = 0; i < genreNode.Count; i++)
                {
                    genres[i] = new DataBase(genreNode[i].SelectSingleNode("./span")?.InnerText, new Uri(BaseUrl, genreNode[i].Attributes["href"]?.Value));
                }
            }
            else
                genres = new DataBase[0];

            return genres;
        }

        private Name GetName(HtmlNode infoNode)
        {
            var orName = Decode(infoNode?.SelectSingleNode("./tr[1]/td[2]/h2[@class='aname']")?.InnerText);
            var engName = Decode(infoNode?.SelectSingleNode("./tr[2]/td[2]")?.InnerText);

            if (orName != null || engName != null)
            {
                return new Name(null, engName == String.Empty ? null : engName, orName == String.Empty ? null : orName);
            }
            else
                return new Name();
        }

        #endregion Data Getting Methods

        /// <summary>
        /// Cause mangareader/mangapanda redirect from https to http image
        /// </summary>
        /// <param name="coverUri">Cover link</param>
        /// <returns></returns>
        private string FixCoverProtocol(string coverUri)
        {
            return coverUri.Contains("https") ? coverUri.Replace("https", "http") : coverUri;
        }

        #endregion Private Methods

        #endregion Methods
    }
}