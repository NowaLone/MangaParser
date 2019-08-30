using HtmlAgilityPack;
using MangaParser.Core.Interfaces;
using MangaParser.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MangaParser.Parsers.Mangareader
{
    public class MangareaderParser : Parser
    {
        #region Constructors

        public MangareaderParser(string baseUri = "https://www.mangareader.net") : base(baseUri)
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

            var mainNode = htmlDoc.DocumentNode.SelectSingleNode("//div[@id='wrapper_body']");

            return GetChapters(mainNode);
        }

        public override IManga GetManga(string mangaUri)
        {
            if (!Uri.IsWellFormedUriString(mangaUri, UriKind.Absolute))
                throw new UriFormatException($"Manga uri is wrong: {mangaUri}");

            if (!mangaUri.Contains(BaseUri.Host))
                return null;

            var htmlDoc = Web.Load(mangaUri);

            var mainNode = htmlDoc.DocumentNode.SelectSingleNode("//div[@id='wrapper_body']");

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

            var mainNode = htmlDoc.DocumentNode.SelectSingleNode("//body/div[@id='container']");

            foreach (var item in GetMangaPages(mainNode))
            {
                yield return item;
            }
        }

        public override IEnumerable<IMangaThumb> SearchManga(string query)
        {
            query = query.Replace(' ', '+');

            var htmlDoc = Web.Load(BaseUri + $"search/?w={query}");

            var mainNode = htmlDoc.DocumentNode.SelectSingleNode("//div[@id='mangaresults']");

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

            var mainNode = htmlDoc.DocumentNode.SelectSingleNode("//div[@id='wrapper_body']");

            return GetChapters(mainNode);
        }

        public override async Task<IManga> GetMangaAsync(string mangaUri)
        {
            if (!Uri.IsWellFormedUriString(mangaUri, UriKind.Absolute))
                throw new UriFormatException($"Manga uri is wrong: {mangaUri}");

            if (!mangaUri.Contains(BaseUri.Host))
                return null;

            var htmlDoc = await Web.LoadFromWebAsync(mangaUri);

            var mainNode = htmlDoc.DocumentNode.SelectSingleNode("//div[@id='wrapper_body']");

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

            var mainNode = htmlDoc.DocumentNode.SelectSingleNode("//body/div[@id='container']");

            return await GetMangaPagesAsync(mainNode);
        }

        public override async Task<IEnumerable<IMangaThumb>> SearchMangaAsync(string query)
        {
            query = query.Replace(' ', '+');

            var htmlDoc = await Web.LoadFromWebAsync(BaseUri + $"search/?w={query}");

            var mainNode = htmlDoc.DocumentNode.SelectSingleNode("//div[@id='mangaresults']");

            return GetSearchResult(mainNode);
        }

        #endregion Asynchronous Methods

        #endregion Public Methods

        #region Private Methods

        private MangareaderObject GetMangaData(HtmlNode mainNode)
        {
            var infoNode = mainNode?.SelectSingleNode("//div[@id='mangaproperties']/table");

            var manga = new MangareaderObject
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
            string imgNodePath = mainNode.XPath + "//img[@id='img']";

            var Counters = mainNode.SelectNodes("./div[@id='topchapter']/div[@id='navi']/div[@id='selectpage']/select/option");

            MangaPage[] pages;

            if (Counters != null)
            {
                pages = new MangaPage[Counters.Count];

                pages[0] = new MangaPage(mainNode.SelectSingleNode("//img[@id='img']")?.Attributes["src"]?.Value);

                HtmlDocument htmlDoc;

                for (int i = 1; i < Counters.Count; i++)
                {
                    htmlDoc = await Web.LoadFromWebAsync(new Uri(BaseUri, Counters[i].Attributes["value"]?.Value).OriginalString);

                    pages[i] = new MangaPage(htmlDoc.DocumentNode.SelectSingleNode(imgNodePath)?.Attributes["src"]?.Value);
                }
            }
            else
                pages = new MangaPage[0];

            return pages;
        }

        private IEnumerable<MangaPage> GetMangaPages(HtmlNode mainNode)
        {
            string imgNodePath = mainNode.XPath + "//img[@id='img']";

            var Counters = mainNode.SelectNodes("./div[@id='topchapter']/div[@id='navi']/div[@id='selectpage']/select/option");

            if (Counters != null)
            {
                yield return new MangaPage(mainNode.SelectSingleNode("//img[@id='img']")?.Attributes["src"]?.Value);

                HtmlDocument htmlDoc;

                for (int i = 1; i < Counters.Count; i++)
                {
                    htmlDoc = Web.Load(new Uri(BaseUri, Counters[i].Attributes["value"]?.Value));

                    yield return new MangaPage(htmlDoc.DocumentNode.SelectSingleNode(imgNodePath)?.Attributes["src"]?.Value);
                }
            }
        }

        #region Search methods

        private IEnumerable<MangareaderObject> GetSearchResult(HtmlNode mainNode)
        {
            var thumbs = mainNode?.SelectNodes("//div[@class='mangaresultinner']");

            if (thumbs != null)
            {
                for (int i = 0; i < thumbs.Count; i++)
                {
                    string coverstr = thumbs[i].SelectSingleNode("./div[@class='imgsearchresults']")?.Attributes["style"]?.Value;

                    var manga = new MangareaderObject
                    {
                        Genres = GetThumbGenres(thumbs[i].SelectSingleNode("./div[@class='result_info c5']/div[@class='manga_genre']")),
                        Covers = new MangaCover[] { new MangaCover(coverstr.Substring(coverstr.IndexOf("('") + 2, coverstr.IndexOf("')") - coverstr.IndexOf("('") - 2)) },
                        MangaUri = new Uri(BaseUri, thumbs[i].SelectSingleNode("./div[@class='result_info c5']/div[@class='manga_name']/div/h3/a")?.Attributes["href"]?.Value),
                        Name = new MangaName(null, null, thumbs[i].SelectSingleNode("./div[@class='result_info c5']/div[@class='manga_name']/div/h3/a")?.InnerText),
                    };

                    yield return manga;
                }
            }
        }

        private MangaDataBase[] GetThumbGenres(HtmlNode node)
        {
            var genres = node?.InnerText.Replace(" ", String.Empty).Split(',');

            MangaDataBase[] Genres;

            if (genres != null)
            {
                Genres = new MangaDataBase[genres.Length];

                for (int i = 0; i < genres.Length; i++)
                {
                    Genres[i] = new MangaDataBase(genres[i], default(Uri));
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
            var chapters = mainNode.SelectNodes("//div[@id='chapterlist']/table/tr");

            MangaChapter[] Chapters;

            if (chapters != null)
            {
                Chapters = new MangaChapter[chapters.Count - 1];

                for (int i = 1; i < chapters.Count; i++)
                {
                    DateTime.TryParseExact(Decode(chapters[i].SelectSingleNode("./td[2]")?.InnerText), "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime date);

                    var name = Decode(chapters[i].SelectSingleNode("./td/a")?.InnerText);
                    var additional = Decode(chapters[i].SelectSingleNode("./td/text()[3]")?.InnerText);

                    Chapters[i - 1] = new MangaChapter(name + (additional == ":" ? String.Empty : additional), new Uri(BaseUri, chapters[i].SelectSingleNode("./td/a")?.Attributes["href"]?.Value), date);
                }
            }
            else
            {
                Chapters = new MangaChapter[0];
            }

            return Chapters;
        }

        private MangaDataBase[] GetInfoData(HtmlNode infoNode, int trNumber)
        {
            var data = infoNode?.SelectSingleNode($"./tr[{trNumber}]/td[2]");

            MangaDataBase[] Data;

            if (data != null)
            {
                Data = new MangaDataBase[1];

                Data[0] = new MangaDataBase(Decode(data?.InnerText), default(Uri));
            }
            else
            {
                Data = new MangaDataBase[0];
            }

            return Data;
        }

        private MangaCover[] GetCovers(HtmlNode mainNode)
        {
            var imageNode = mainNode?.SelectSingleNode("//img");

            MangaCover[] images;

            if (imageNode != null)
            {
                images = new MangaCover[] { new MangaCover(null, null, imageNode.Attributes["src"]?.Value) };
            }
            else
                images = new MangaCover[0];

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

        private MangaDataBase[] GetGenres(HtmlNode infoNode)
        {
            var genreNode = infoNode?.SelectNodes("./tr[8]//a");

            MangaDataBase[] genres;

            if (genreNode != null)
            {
                genres = new MangaDataBase[genreNode.Count];

                for (int i = 0; i < genreNode.Count; i++)
                {
                    genres[i] = new MangaDataBase(genreNode[i].SelectSingleNode("./span")?.InnerText, new Uri(BaseUri, genreNode[i].Attributes["href"]?.Value));
                }
            }
            else
                genres = new MangaDataBase[0];

            return genres;
        }

        private MangaName GetName(HtmlNode infoNode)
        {
            var orName = Decode(infoNode?.SelectSingleNode("./tr[1]/td[2]/h2[@class='aname']")?.InnerText);
            var engName = Decode(infoNode?.SelectSingleNode("./tr[2]/td[2]")?.InnerText);

            if (orName != null || engName != null)
            {
                return new MangaName(engName, null, orName);
            }
            else
                return new MangaName();
        }

        #endregion Data Getting Methods

        #endregion Private Methods
    }
}