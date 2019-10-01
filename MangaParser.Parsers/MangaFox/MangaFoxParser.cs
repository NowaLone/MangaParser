using HtmlAgilityPack;
using MangaParser.Core.Interfaces;
using MangaParser.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MangaParser.Parsers.MangaFox
{
    public class MangaFoxParser : Parser
    {
        #region Constructors

        public MangaFoxParser(string baseUri = "http://fanfox.net") : base(baseUri)
        {
        }

        #endregion Constructors

        #region Properties

        protected static HtmlWeb Web { get; } = new HtmlWeb();

        #endregion Properties

        #region Public Methods

        #region Synchronous Methods

        public override IEnumerable<IChapter> GetChapters(string mangaUri)
        {
            if (!Uri.IsWellFormedUriString(mangaUri, UriKind.Absolute))
                throw new UriFormatException($"Manga uri is wrong: {mangaUri}");

            if (!mangaUri.Contains(BaseUri.Host))
                return null;

            UriBuilder uriBuilder = new UriBuilder(mangaUri);

            if (!uriBuilder.Uri.Host.Contains("m."))
                uriBuilder.Host = "m." + uriBuilder.Host;

            mangaUri = uriBuilder.Uri.OriginalString;

            var htmlDoc = Web.Load(mangaUri);

            var mainNode = htmlDoc.DocumentNode.SelectSingleNode("/html/body/div[1]/section/div/div[3]/div[1]");

            Web.PreRequest = null;

            return GetChapters(mainNode);
        }

        public override IManga GetManga(string mangaUri)
        {
            if (!Uri.IsWellFormedUriString(mangaUri, UriKind.Absolute))
                throw new UriFormatException($"Manga uri is wrong: {mangaUri}");

            if (!mangaUri.Contains(BaseUri.Host))
                return null;

            UriBuilder uriBuilder = new UriBuilder(mangaUri);

            if (uriBuilder.Uri.Host.Contains("m."))
                uriBuilder.Host = uriBuilder.Host.Replace("m.", String.Empty);

            mangaUri = uriBuilder.Uri.OriginalString;

            var htmlDoc = Web.Load(mangaUri);

            var mainNode = htmlDoc.DocumentNode.SelectSingleNode("//body/div[@class='container']/div[@class='detail-info']");

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

            UriBuilder uriBuilder = new UriBuilder(chapterUri);

            if (!uriBuilder.Uri.Host.Contains("m."))
                uriBuilder.Host = "m." + uriBuilder.Host;

            chapterUri = uriBuilder.Uri.OriginalString;

            var htmlDoc = Web.Load(chapterUri);

            var mainNode = htmlDoc.DocumentNode.SelectSingleNode("//body/div[@class='site-content']/section[@class='main']/div[@class='mangaread-main']");

            foreach (var item in GetMangaPages(mainNode))
            {
                yield return item;
            }
        }

        public override IEnumerable<IMangaThumb> SearchManga(string query)
        {
            query = query.Replace(' ', '+');

            var htmlDoc = Web.Load(BaseUri + $"/search?name={query}");

            var mainNode = htmlDoc.DocumentNode.SelectSingleNode("//body/div[@class='container']/div[@class='line-list']/div[@class='manga-list-4 mt15']/ul[@class='manga-list-4-list line']");

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

            UriBuilder uriBuilder = new UriBuilder(mangaUri);

            if (!uriBuilder.Uri.Host.Contains("m."))
                uriBuilder.Host = "m." + uriBuilder.Host;

            mangaUri = uriBuilder.Uri.OriginalString;

            var htmlDoc = await Web.LoadFromWebAsync(mangaUri);

            var mainNode = htmlDoc.DocumentNode.SelectSingleNode("/html/body/div[1]/section/div/div[3]/div[1]");

            Web.PreRequest = null;

            return GetChapters(mainNode);
        }

        public override async Task<IManga> GetMangaAsync(string mangaUri)
        {
            if (!Uri.IsWellFormedUriString(mangaUri, UriKind.Absolute))
                throw new UriFormatException($"Manga uri is wrong: {mangaUri}");

            if (!mangaUri.Contains(BaseUri.Host))
                return null;

            var htmlDoc = await Web.LoadFromWebAsync(mangaUri);

            var mainNode = htmlDoc.DocumentNode.SelectSingleNode("//body/div[@class='container']/div[@class='detail-info']");

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

            UriBuilder uriBuilder = new UriBuilder(chapterUri);

            if (!uriBuilder.Uri.Host.Contains("m."))
                uriBuilder.Host = "m." + uriBuilder.Host;

            chapterUri = uriBuilder.Uri.OriginalString;

            var htmlDoc = await Web.LoadFromWebAsync(chapterUri);

            var mainNode = htmlDoc.DocumentNode.SelectSingleNode("//body/div[@class='site-content']/section[@class='main']/div[@class='mangaread-main']");

            return await GetMangaPagesAsync(mainNode);
        }

        public override async Task<IEnumerable<IMangaThumb>> SearchMangaAsync(string query)
        {
            query = query.Replace(' ', '+');

            var htmlDoc = await Web.LoadFromWebAsync(BaseUri + $"/search?name={query}");

            var mainNode = htmlDoc.DocumentNode.SelectSingleNode("//body/div[@class='container']/div[@class='line-list']/div[@class='manga-list-4 mt15']/ul[@class='manga-list-4-list line']");

            return GetSearchResult(mainNode);
        }

        #endregion Asynchronous Methods

        #endregion Public Methods

        #region Private Methods

        private MangaChapter[] GetChapters(HtmlNode mainNode)
        {
            var chapters = mainNode?.SelectNodes(".//a");

            MangaChapter[] Chapters;

            if (chapters != null)
            {
                Chapters = new MangaChapter[chapters.Count];

                for (int i = chapters.Count - 1; i >= 0; i--)
                {
                    DateTime.TryParse(Decode(chapters[i].SelectSingleNode("./span")?.InnerText), out DateTime date);

                    Chapters[chapters.Count - 1 - i] = new MangaChapter(Decode(chapters[i].InnerText), "http:" + chapters[i].Attributes["href"]?.Value, date);
                }
            }
            else
            {
                Chapters = new MangaChapter[0];
            }

            return Chapters;
        }

        private MangaObject GetMangaData(HtmlNode mainNode)
        {
            var infoNode = mainNode?.SelectSingleNode(".//div[@class='detail-info-right']");

            var manga = new MangaObject
            {
                Name = GetName(infoNode),
                Description = GetDescription(infoNode),
                Covers = GetCovers(mainNode),
                Genres = GetGenres(infoNode),
                Autors = GetAutors(infoNode),
            };

            return manga;
        }

        private async Task<MangaPage[]> GetMangaPagesAsync(HtmlNode mainNode)
        {
            string imgNodePath = mainNode.XPath + "/div[@class='mangaread-img']/a/img";

            var Counters = mainNode.SelectNodes("./div[@class='mangaread-operate  clearfix']/select/option");

            MangaPage[] pages;

            if (Counters != null)
            {
                pages = new MangaPage[Counters.Count];

                pages[0] = new MangaPage(mainNode.SelectSingleNode("./div[@class='mangaread-img']/a/img")?.Attributes["src"]?.Value);

                HtmlDocument htmlDoc;

                for (int i = 1; i < Counters.Count; i++)
                {
                    htmlDoc = await Web.LoadFromWebAsync("http:" + Counters[i].Attributes["value"]?.Value);

                    pages[i] = new MangaPage(htmlDoc.DocumentNode.SelectSingleNode(imgNodePath)?.Attributes["src"]?.Value);
                }
            }
            else
                pages = new MangaPage[0];

            return pages;
        }

        private IEnumerable<MangaPage> GetMangaPages(HtmlNode mainNode)
        {
            string imgNodePath = mainNode.XPath + "/div[@class='mangaread-img']/a/img";

            var Counters = mainNode.SelectNodes("./div[@class='mangaread-operate  clearfix']/select/option");

            if (Counters != null)
            {
                yield return new MangaPage(mainNode.SelectSingleNode("./div[@class='mangaread-img']/a/img")?.Attributes["src"]?.Value);

                HtmlDocument htmlDoc;

                for (int i = 1; i < Counters.Count; i++)
                {
                    htmlDoc = Web.Load("http:" + Counters[i].Attributes["value"]?.Value);

                    yield return new MangaPage(htmlDoc.DocumentNode.SelectSingleNode(imgNodePath)?.Attributes["src"]?.Value);
                }
            }
        }

        private IEnumerable<MangaObject> GetSearchResult(HtmlNode mainNode)
        {
            var thumbs = mainNode?.SelectNodes("./li");

            if (thumbs != null)
            {
                for (int i = 0; i < thumbs.Count; i++)
                {
                    var manga = new MangaObject
                    {
                        Autors = new MangaDataBase[] { new MangaDataBase(thumbs[i].SelectSingleNode("./p[@class='manga-list-4-item-tip']/a")?.Attributes["title"]?.Value, thumbs[i].SelectSingleNode("./p[@class='manga-list-4-item-tip']/a")?.Attributes["href"]?.Value) },
                        Genres = new MangaDataBase[] { new MangaDataBase(Decode(thumbs[i].SelectSingleNode("./p[@class='manga-list-4-show-tag-list-2']/a")?.InnerText), default(Uri)) },
                        Covers = new MangaCover[] { new MangaCover(null, null, thumbs[i].SelectSingleNode("./a/img")?.Attributes["src"]?.Value) },
                        MangaUri = new Uri(BaseUri, thumbs[i].SelectSingleNode("./a")?.Attributes["href"]?.Value),
                        Description = Decode(thumbs[i].LastChild?.InnerText),
                        Name = new MangaName(thumbs[i].SelectSingleNode("./a")?.Attributes["title"]?.Value, null, null),
                    };

                    yield return manga;
                }
            }
        }

        #region Data Getting Methods

        private MangaDataBase[] GetAutors(HtmlNode infoNode)
        {
            var autorsNode = infoNode?.SelectNodes("./p[@class='detail-info-right-say']/a");

            MangaDataBase[] autors;

            if (autorsNode != null)
            {
                autors = new MangaDataBase[autorsNode.Count];

                for (int i = 0; i < autorsNode.Count; i++)
                {
                    autors[i] = new MangaDataBase(autorsNode[i].Attributes["title"]?.Value, autorsNode[i].Attributes["href"]?.Value);
                }
            }
            else
                autors = new MangaDataBase[0];

            return autors;
        }

        private MangaCover[] GetCovers(HtmlNode mainNode)
        {
            var imageNode = mainNode?.SelectSingleNode("./div[@class='detail-info-cover']/img");

            MangaCover[] images;

            if (imageNode != null)
            {
                images = new MangaCover[] { new MangaCover(null, null, imageNode.Attributes["src"]?.Value) };
            }
            else
                images = new MangaCover[0];

            return images;
        }

        private string GetDescription(HtmlNode infoNode)
        {
            var descNode = infoNode?.SelectSingleNode("./p[@class='fullcontent']");

            if (descNode != null)
            {
                return Decode(descNode.InnerText);
            }
            else
                return null;
        }

        private MangaDataBase[] GetGenres(HtmlNode infoNode)
        {
            var genreNode = infoNode?.SelectNodes("./p[@class='detail-info-right-tag-list']/a");

            MangaDataBase[] genres;

            if (genreNode != null)
            {
                genres = new MangaDataBase[genreNode.Count];

                for (int i = 0; i < genreNode.Count; i++)
                {
                    genres[i] = new MangaDataBase(genreNode[i].Attributes["title"]?.Value, genreNode[i].Attributes["href"]?.Value);
                }
            }
            else
                genres = new MangaDataBase[0];

            return genres;
        }

        private MangaName GetName(HtmlNode infoNode)
        {
            var nameNode = infoNode?.SelectSingleNode("./p[@class='detail-info-right-title']/span[@class='detail-info-right-title-font']");

            if (nameNode != null)
            {
                return new MangaName(Decode(nameNode.InnerText), null, null);
            }
            else
                return new MangaName();
        }

        #endregion Data Getting Methods

        #endregion Private Methods
    }
}