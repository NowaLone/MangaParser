using HtmlAgilityPack;
using MangaParser.Core.Interfaces;
using MangaParser.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MangaParser.Parsers.MangaFox
{
    public class MangaFoxParser : Parser, IParser
    {
        public MangaFoxParser(string baseUri = "http://fanfox.net") : base(baseUri)
        {
        }

        #region Public Methods

        #region Synchronous Methods

        public IEnumerable<IChapter> GetChapters(IMangaThumb manga)
        {
            return GetChaptersAsync(manga).Result;
        }

        public IEnumerable<IChapter> GetChapters(string mangaUri)
        {
            return GetChaptersAsync(mangaUri).Result;
        }

        public IEnumerable<IChapter> GetChapters(Uri mangaUri)
        {
            return GetChaptersAsync(mangaUri).Result;
        }

        public IManga GetManga(IMangaThumb mangaThumb)
        {
            return GetMangaAsync(mangaThumb).Result;
        }

        public IManga GetManga(string mangaUri)
        {
            return GetMangaAsync(mangaUri).Result;
        }

        public IManga GetManga(Uri mangaUri)
        {
            return GetMangaAsync(mangaUri).Result;
        }

        public IEnumerable<IPage> GetPages(IChapter chapter)
        {
            return GetPagesAsync(chapter).Result;
        }

        public IEnumerable<IPage> GetPages(string chapterUri)
        {
            return GetPagesAsync(chapterUri).Result;
        }

        public IEnumerable<IPage> GetPages(Uri chapterUri)
        {
            return GetPagesAsync(chapterUri).Result;
        }

        public IEnumerable<IMangaThumb> SearchManga(string query)
        {
            return SearchMangaAsync(query).Result;
        }

        #endregion Synchronous Methods

        #region Asynchronous Methods

        public async Task<IEnumerable<IMangaThumb>> SearchMangaAsync(string query)
        {
            query = query.Replace(' ', '+');

            var htmlDoc = await Web.LoadFromWebAsync(BaseUri + $"/search?name={query}");

            var mainNode = htmlDoc.DocumentNode.SelectSingleNode("//body/div[@class='container']/div[@class='line-list']/div[@class='manga-list-4 mt15']/ul[@class='manga-list-4-list line']");

            return GetSearchResult(mainNode);
        }

        public async Task<IEnumerable<IChapter>> GetChaptersAsync(IMangaThumb manga)
        {
            return await GetChaptersAsync(manga.MangaUri.OriginalString);
        }

        public async Task<IEnumerable<IChapter>> GetChaptersAsync(string mangaUri)
        {
                if (!mangaUri.Contains(BaseUri.Host))
                    return null;

                UriBuilder uriBuilder = new UriBuilder(mangaUri);

                if (!uriBuilder.Uri.Host.Contains("m."))
                    uriBuilder.Host = "m." + uriBuilder.Host;

                mangaUri = uriBuilder.Uri.OriginalString;

                var htmlDoc =await Web.LoadFromWebAsync(mangaUri);

                var mainNode = htmlDoc.DocumentNode.SelectSingleNode("/html/body/div[1]/section/div/div[3]/div[1]");

                Web.PreRequest = null;

                return GetChapters(mainNode);
        }

        public async Task<IEnumerable<IChapter>> GetChaptersAsync(Uri mangaUri)
        {
            return await GetChaptersAsync(mangaUri.OriginalString);
        }

        public async Task<IManga> GetMangaAsync(IMangaThumb mangaThumb)
        {
            return await GetMangaAsync(mangaThumb.MangaUri.OriginalString);
        }

        public async Task<IManga> GetMangaAsync(string mangaUri)
        {
            if (!mangaUri.Contains(BaseUri.Host))
                return null;

            var htmlDoc = await Web.LoadFromWebAsync(mangaUri);

            var mainNode = htmlDoc.DocumentNode.SelectSingleNode("//body/div[@class='container']/div[@class='detail-info']");

            var manga = GetMangaData(mainNode);

            manga.MangaUri = new Uri(mangaUri);

            return manga;
        }

        public async Task<IManga> GetMangaAsync(Uri mangaUri)
        {
            return await GetMangaAsync(mangaUri.OriginalString);
        }

        public async Task<IEnumerable<IPage>> GetPagesAsync(IChapter chapter)
        {
            return await GetPagesAsync(chapter.ChapterUri.OriginalString);
        }

        public async Task<IEnumerable<IPage>> GetPagesAsync(string chapterUri)
        {
            if (!chapterUri.Contains(BaseUri.Host))
                return null;

            UriBuilder uriBuilder = new UriBuilder(chapterUri);

            if (!uriBuilder.Uri.Host.Contains("m."))
                uriBuilder.Host = "m." + uriBuilder.Host;

            chapterUri = uriBuilder.Uri.OriginalString;

            var htmlDoc = await Web.LoadFromWebAsync(chapterUri);

            var mainNode = htmlDoc.DocumentNode.SelectSingleNode("//body/div[@class='site-content']/section[@class='main']/div[@class='mangaread-main']");

            return await GetMangaPages(mainNode);
        }

        public async Task<IEnumerable<IPage>> GetPagesAsync(Uri chapterUri)
        {
            return await GetPagesAsync(chapterUri.OriginalString);
        }

        #endregion Asynchronous Methods

        #endregion Public Methods

        #region Private Methods

        private IEnumerable<MangaFoxObject> GetSearchResult(HtmlNode mainNode)
        {
            var thumbs = mainNode?.SelectNodes("./li");

            if (thumbs != null)
            {
                for (int i = 0; i < thumbs.Count; i++)
                {
                    var manga = new MangaFoxObject
                    {
                        Autors = new MangaDataBase[] { new MangaDataBase(thumbs[i].SelectSingleNode("./p[@class='manga-list-4-item-tip']/a")?.Attributes["title"]?.Value, thumbs[i].SelectSingleNode("./p[@class='manga-list-4-item-tip']/a")?.Attributes["href"]?.Value) },
                        Genres = new MangaDataBase[] { new MangaDataBase(Decode(thumbs[i].SelectSingleNode("./p[@class='manga-list-4-show-tag-list-2']/a")?.InnerText), default(Uri)) },
                        Covers = new MangaCover[] { new MangaCover(null, null, thumbs[i].SelectSingleNode("./a/img")?.Attributes["src"]?.Value) },
                        MangaUri = new Uri(BaseUri, thumbs[i].SelectSingleNode("./a")?.Attributes["href"]?.Value),
                        Description = Decode(thumbs[i].LastChild?.InnerText),
                        Name = new MangaName(null, thumbs[i].SelectSingleNode("./a")?.Attributes["title"]?.Value, null),
                    };

                    yield return manga;
                }
            }
        }

        private MangaFoxObject GetMangaData(HtmlNode mainNode)
        {
            var infoNode = mainNode?.SelectSingleNode(".//div[@class='detail-info-right']");

            var manga = new MangaFoxObject
            {
                Name = GetName(infoNode),
                Description = GetDescription(infoNode),
                Covers = GetCovers(mainNode),
                Genres = GetGenres(infoNode),
                Autors = GetAutors(infoNode),
            };

            return manga;
        }

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

                    Chapters[chapters.Count - 1 - i] = new MangaChapter(Decode(chapters[i].InnerText), "http:"+ chapters[i].Attributes["href"]?.Value, date);
                }
            }
            else
            {
                Chapters = new MangaChapter[0];
            }

            return Chapters;
        }

        private async Task<MangaPage[]> GetMangaPages(HtmlNode mainNode)
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

        #region Data Getting Methods

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

        private MangaName GetName(HtmlNode infoNode)
        {
            var nameNode = infoNode?.SelectSingleNode("./p[@class='detail-info-right-title']/span[@class='detail-info-right-title-font']");

            if (nameNode != null)
            {
                return new MangaName(null, Decode(nameNode.InnerText), null);
            }
            else
                return new MangaName();
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

        #endregion Data Getting Methods

        #endregion Private Methods
    }
}