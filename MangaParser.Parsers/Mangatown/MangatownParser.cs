using HtmlAgilityPack;
using MangaParser.Core.Interfaces;
using MangaParser.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MangaParser.Parsers.Mangatown
{
    public class MangatownParser : Parser
    {
        #region Constructors

        public MangatownParser(string baseUri = "https://www.mangatown.com") : base(baseUri)
        {
        }

        #endregion Constructors

        #region Public Methods

        #region Synchronous Methods

        public override IEnumerable<IChapter> GetChapters(string mangaUri)
        {
            if (!Uri.IsWellFormedUriString(mangaUri, UriKind.Absolute))
                throw new UriFormatException($"Manga uri is wrong: {mangaUri}");

            if (!mangaUri.Contains(BaseUri.Host))
                return null;

            var htmlDoc = Web.Load(mangaUri);

            var mainNode = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='chapter_content']");

            return GetChapters(mainNode);
        }

        public override IManga GetManga(string mangaUri)
        {
            if (!Uri.IsWellFormedUriString(mangaUri, UriKind.Absolute))
                throw new UriFormatException($"Manga uri is wrong: {mangaUri}");

            if (!mangaUri.Contains(BaseUri.Host))
                return null;

            var htmlDoc = Web.Load(mangaUri);

            var mainNode = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='article_content']");

            var manga = GetMangaData(mainNode);

            manga.MangaUri = new Uri(mangaUri);

            return manga;
        }

        public override IEnumerable<IPage> GetPages(string chapterUri)
        {
            if (!Uri.IsWellFormedUriString(chapterUri, UriKind.Absolute))
                throw new UriFormatException($"Chapter uri is wrong: {chapterUri}");

            if (!chapterUri.Contains(BaseUri.Host))
                yield return null;

            var htmlDoc = Web.Load(chapterUri);

            var mainNode = htmlDoc.DocumentNode.SelectSingleNode("/html/body/section");

            foreach (var item in GetMangaPages(mainNode))
            {
                yield return item;
            }
        }

        public override IEnumerable<IMangaThumb> SearchManga(string query)
        {
            query = query.Replace(' ', '+');

            var htmlDoc = Web.Load(BaseUri + $"/search.php?name={query}");

            var mainNode = htmlDoc.DocumentNode.SelectSingleNode("/html/body/section/article/div/div[2]/ul");

            foreach (var item in GetSearchResult(mainNode))
            {
                yield return item;
            }
        }

        #endregion Synchronous Methods

        #region Asynchronous Methods

        public override async Task<IEnumerable<IChapter>> GetChaptersAsync(string mangaUri)
        {
            if (!Uri.IsWellFormedUriString(mangaUri, UriKind.Absolute))
                throw new UriFormatException($"Manga uri is wrong: {mangaUri}");

            if (!mangaUri.Contains(BaseUri.Host))
                return null;

            var htmlDoc = await Web.LoadFromWebAsync(mangaUri);

            var mainNode = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='chapter_content']");

            return GetChapters(mainNode);
        }

        public override async Task<IManga> GetMangaAsync(string mangaUri)
        {
            if (!Uri.IsWellFormedUriString(mangaUri, UriKind.Absolute))
                throw new UriFormatException($"Manga uri is wrong: {mangaUri}");

            if (!mangaUri.Contains(BaseUri.Host))
                return null;

            var htmlDoc = await Web.LoadFromWebAsync(mangaUri);

            var mainNode = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='article_content']");

            var manga = GetMangaData(mainNode);

            manga.MangaUri = new Uri(mangaUri);

            return manga;
        }

        public override async Task<IEnumerable<IPage>> GetPagesAsync(string chapterUri)
        {
            if (!Uri.IsWellFormedUriString(chapterUri, UriKind.Absolute))
                throw new UriFormatException($"Chapter uri is wrong: {chapterUri}");

            if (!chapterUri.Contains(BaseUri.Host))
                return null;

            var htmlDoc = await Web.LoadFromWebAsync(chapterUri);

            var mainNode = htmlDoc.DocumentNode.SelectSingleNode("/html/body/section");

            return await GetMangaPagesAsync(mainNode);
        }

        public override async Task<IEnumerable<IMangaThumb>> SearchMangaAsync(string query)
        {
            query = query.Replace(' ', '+');

            var htmlDoc = await Web.LoadFromWebAsync(BaseUri + $"/search.php?name={query}");

            var mainNode = htmlDoc.DocumentNode.SelectSingleNode("/html/body/section/article/div/div[2]/ul");

            return GetSearchResult(mainNode);
        }

        #endregion Asynchronous Methods

        #endregion Public Methods

        #region Private Methods

        private MangatownObject GetMangaData(HtmlNode mainNode)
        {
            var infoNode = mainNode?.SelectSingleNode(".//div[@class='detail_info clearfix']/ul");

            var manga = new MangatownObject
            {
                Name = GetName(mainNode),
                Description = GetDescription(mainNode),
                Covers = new MangaCover[] { new MangaCover(null, null, new Uri(mainNode?.SelectSingleNode(".//img").Attributes["src"].Value)) },
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
                    htmlDoc = await Web.LoadFromWebAsync(new Uri(BaseUri, Counters[i].Attributes["value"]?.Value).OriginalString);

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
                    htmlDoc = Web.Load(new Uri(BaseUri, Counters[i].Attributes["value"]?.Value));

                    yield return new MangaPage(htmlDoc.DocumentNode.SelectSingleNode(".//img[@id='image']")?.Attributes["src"]?.Value);
                }
            }
        }

        #region Search methods

        private IEnumerable<MangatownObject> GetSearchResult(HtmlNode mainNode)
        {
            var thumbs = mainNode?.SelectNodes("./li");

            if (thumbs != null)
            {
                for (int i = 0; i < thumbs.Count; i++)
                {
                    var manga = new MangatownObject
                    {
                        Autors = new MangaDataBase[] { new MangaDataBase(Decode(thumbs[i].SelectSingleNode("./p[4]/a")?.InnerText), thumbs[i].SelectSingleNode("./p[4]/a")?.Attributes["href"]?.Value) },
                        Genres = GetThumbGenres(thumbs[i].SelectSingleNode("./p[3]")),
                        Covers = new MangaCover[] { new MangaCover(null, null, thumbs[i].SelectSingleNode("./a/img")?.Attributes["src"]?.Value) },
                        MangaUri = new Uri(BaseUri, thumbs[i].SelectSingleNode("./a")?.Attributes["href"]?.Value),
                        Name = new MangaName(null, null, thumbs[i].SelectSingleNode("./a")?.Attributes["title"]?.Value),
                    };

                    yield return manga;
                }
            }
        }

        private MangaDataBase[] GetThumbGenres(HtmlNode node)
        {
            var genres = node?.SelectNodes("./a");

            MangaDataBase[] Genres;

            if (genres != null)
            {
                Genres = new MangaDataBase[genres.Count];

                for (int i = 0; i < genres.Count; i++)
                {
                    Genres[i] = new MangaDataBase(Decode(genres[i].InnerText), new Uri(BaseUri, genres[i].Attributes["href"].Value));
                }
            }
            else
            {
                Genres = new MangaDataBase[0];
            }

            return Genres;
        }

        #endregion Search methods

        #region Data Getting Methods

        private MangaChapter[] GetChapters(HtmlNode mainNode)
        {
            var chapters = mainNode?.SelectNodes("./ul/li");

            MangaChapter[] Chapters;

            if (chapters != null)
            {
                Chapters = new MangaChapter[chapters.Count];

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

                    Chapters[chapters.Count - 1 - i] = new MangaChapter(name + additional, new Uri(BaseUri, chapters[i].SelectSingleNode("./a")?.Attributes["href"]?.Value), date);
                }
            }
            else
            {
                Chapters = new MangaChapter[0];
            }

            return Chapters;
        }

        private MangaDataBase[] GetInfoData(HtmlNode infoNode, int liNumber)
        {
            var data = infoNode?.SelectNodes($"./li[{liNumber}]/a");

            MangaDataBase[] Data;

            if (data != null)
            {
                Data = new MangaDataBase[data.Count];

                for (int i = 0; i < data.Count; i++)
                {
                    Data[i] = new MangaDataBase(Decode(data[i]?.InnerText), new Uri(BaseUri, data[i].Attributes["href"].Value));
                }
            }
            else
            {
                Data = new MangaDataBase[0];
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

        private MangaName GetName(HtmlNode mainNode)
        {
            var orName = Decode(mainNode?.SelectSingleNode(".//div[@class='detail_info clearfix']/ul/li[3]/text()")?.InnerText).Split(';')[0];
            var engName = Decode(mainNode?.SelectSingleNode("./h1")?.InnerText);

            if (orName != null || engName != null)
            {
                return new MangaName(engName == String.Empty ? null : engName, null, orName == String.Empty ? null : orName);
            }
            else
                return new MangaName();
        }

        #endregion Data Getting Methods

        #endregion Private Methods
    }
}