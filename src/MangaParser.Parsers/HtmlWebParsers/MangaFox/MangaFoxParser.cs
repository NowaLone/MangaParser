using HtmlAgilityPack;
using MangaParser.Core.Exceptions;
using MangaParser.Core.Interfaces;
using MangaParser.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MangaParser.Parsers.HtmlWebParsers.MangaFox
{
    public class MangaFoxParser : CoreParser
    {
        #region Constructors

        public MangaFoxParser(string baseUri = "https://fanfox.net") : base(baseUri)
        {
        }

        #endregion Constructors

        #region Methods

        #region Search

        public override IEnumerable<IMangaObject> SearchManga(string query)
        {
            query = query is null ? String.Empty : query.Replace(' ', '+');

            var htmlDoc = Web.Load(BaseUrl + $"/search?name={query}");

            return SearchMangaCore(htmlDoc);
        }

        protected override IEnumerable<IMangaObject> SearchMangaCore(HtmlDocument htmlDoc)
        {
            if (htmlDoc is null)
            {
                throw new ArgumentNullException(nameof(htmlDoc));
            }

            var mainNode = htmlDoc.DocumentNode.SelectSingleNode("//body/div[@class='container']/div[@class='line-list']/div[@class='manga-list-4 mt15']/ul[@class='manga-list-4-list line']");

            return GetSearchResult(mainNode);
        }

        #endregion Search

        #region GetManga

        protected override IMangaObject GetMangaCore(HtmlDocument htmlDoc, Uri url)
        {
            if (htmlDoc is null)
            {
                throw new ArgumentNullException(nameof(htmlDoc));
            }

            var mainNode = htmlDoc.DocumentNode.SelectSingleNode("//body/div[@class='container']/div[@class='detail-info']");

            return GetMangaData(mainNode, url);
        }

        #endregion GetManga

        #region GetChapters

        public override IEnumerable<IChapter> GetChapters(Uri url)
        {
            if (url is null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            if (!url.Host.Contains(BaseUrl.Host))
            {
                throw new BaseHostNotMatchException(BaseUrl.Host, url.Host, nameof(url));
            }

            url = PrepareChaptersUrl(url);

            var htmlDoc = Web.Load(url);

            return GetChaptersCore(htmlDoc, url);
        }

        public override async Task<IEnumerable<IChapter>> GetChaptersAsync(Uri url)
        {
            if (url is null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            if (BaseUrl.Host != url.Host)
            {
                throw new BaseHostNotMatchException(BaseUrl.Host, url.Host, nameof(url));
            }

            url = PrepareChaptersUrl(url);

            var htmlDoc = await Web.LoadFromWebAsync(url.OriginalString).ConfigureAwait(false);

            return GetChaptersCore(htmlDoc, url);
        }

        protected override IEnumerable<IChapter> GetChaptersCore(HtmlDocument htmlDoc, Uri url)
        {
            if (htmlDoc is null)
            {
                throw new ArgumentNullException(nameof(htmlDoc));
            }

            var mainNode = htmlDoc.DocumentNode.SelectSingleNode("/html/body/div[1]/section/div/div[3]/div[1]");

            return GetChapters(mainNode);
        }

        #endregion GetChapters

        #region GetPages

        public override IEnumerable<IDataBase> GetPages(Uri url)
        {
            if (url is null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            if (BaseUrl.Host != url.Host)
            {
                throw new BaseHostNotMatchException(BaseUrl.Host, url.Host, nameof(url));
            }

            var preparedUrl = PreparePagesUrl(url);

            var htmlDoc = Web.Load(preparedUrl);

            // Check if we received 'Sorry, it’s licensed and not available.' page 
            if (htmlDoc is null || htmlDoc.DocumentNode is null || htmlDoc.DocumentNode.SelectNodes("/html/body/div[1]/section/div/p") != null)
            {
                // Hack: if we remove a '[page number].html' part from an url, we skip the license block and get all the pages
                var html = Path.GetFileName(preparedUrl.AbsoluteUri);
                var urlWithoutHtml = preparedUrl.AbsoluteUri.Remove(preparedUrl.AbsoluteUri.Length - html.Length, html.Length);
                htmlDoc = Web.Load(urlWithoutHtml);
            }

            return GetPagesCore(htmlDoc, preparedUrl);
        }

        public override async Task<IEnumerable<IDataBase>> GetPagesAsync(Uri url)
        {
            if (url is null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            if (BaseUrl.Host != url.Host)
            {
                throw new BaseHostNotMatchException(BaseUrl.Host, url.Host, nameof(url));
            }

            var preparedUrl = PreparePagesUrl(url);

            var htmlDoc = await Web.LoadFromWebAsync(preparedUrl.OriginalString).ConfigureAwait(false);

            // Check if we received 'Sorry, it’s licensed and not available.' page 
            if (htmlDoc is null || htmlDoc.DocumentNode is null || htmlDoc.DocumentNode.SelectNodes("/html/body/div[1]/section/div/p") != null)
            {
                // Hack: if we remove a '[page number].html' part from an url, we skip the license block and get all the pages
                var html = Path.GetFileName(preparedUrl.AbsoluteUri);
                var urlWithoutHtml = preparedUrl.AbsoluteUri.Remove(preparedUrl.AbsoluteUri.Length - html.Length, html.Length);
                htmlDoc = await Web.LoadFromWebAsync(urlWithoutHtml).ConfigureAwait(false);
            }

            return GetPagesCore(htmlDoc, preparedUrl);
        }

        protected override IEnumerable<IDataBase> GetPagesCore(HtmlDocument htmlDoc, Uri url)
        {
            if (htmlDoc is null)
            {
                throw new ArgumentNullException(nameof(htmlDoc));
            }

            var mainNode = htmlDoc.DocumentNode.SelectSingleNode("//body/div[@class='site-content']/section[@class='main']/div[@class='mangaread-main']");

            return GetMangaPages(mainNode);
        }

        #endregion GetPages

        #region Private Methods

        #region Search methods

        private IEnumerable<IMangaObject> GetSearchResult(HtmlNode mainNode)
        {
            var thumbs = mainNode?.SelectNodes("./li");

            if (thumbs != null)
            {
                for (int i = 0; i < thumbs.Count; i++)
                {
                    Uri url = GetFullUrl(thumbs[i].SelectSingleNode("./p[@class='manga-list-4-item-title']/a")?.Attributes["href"]?.Value);

                    string nameE = Decode(thumbs[i].SelectSingleNode("./p[@class='manga-list-4-item-title']/a")?.Attributes["title"]?.Value);
                    IDataBase<string> nameDataE = new DataBase<string>(nameE, url);
                    IName name = new Name(english: nameDataE);

                    string authorNameE = Decode(thumbs[i].SelectSingleNode("./p[@class='manga-list-4-item-tip']/a")?.Attributes["title"]?.Value);
                    Uri authorUrl = GetFullUrl(Decode(thumbs[i].SelectSingleNode("./p[@class='manga-list-4-item-tip']/a")?.Attributes["href"]?.Value));
                    ICollection<IDataBase<IName>> authors = new DataBase<IName>[] { new DataBase<IName>(new Name(english: authorNameE, url: authorUrl), authorUrl) };

                    DataBase<string> description = new DataBase<string>(Decode(thumbs[i].SelectSingleNode("./p[@class='manga-list-4-item-tip'][last()]")?.InnerText), url);

                    string cover = thumbs[i].SelectSingleNode("./a/img")?.Attributes["src"]?.Value;
                    ICollection<ICover> covers = new ICover[] { new Cover(new DataBase(null, cover)) };

                    var manga = new MangaObject(name, url)
                    {
                        Authors = authors,
                        Covers = covers,
                        Description = description
                    };

                    yield return manga;
                }
            }
        }

        #endregion Search methods

        #region Data getting methods

        private MangaObject GetMangaData(HtmlNode mainNode, Uri uri)
        {
            var infoNode = mainNode?.SelectSingleNode(".//div[@class='detail-info-right']");

            IName name = GetName(infoNode, uri);

            IDataBase<string> description = GetDescription(infoNode, uri);
            ICollection<IDataBase<IName>> genres = GetInfoData(infoNode, "detail-info-right-tag-list");
            ICollection<IDataBase<IName>> authors = GetInfoData(infoNode, "detail-info-right-say");
            ICollection<ICover> covers = GetCovers(mainNode);

            var manga = new MangaObject(name, uri)
            {
                Description = description,
                Genres = genres,
                Authors = authors,
                Covers = covers,
            };

            return manga;
        }

        private ICollection<IDataBase<IName>> GetInfoData(HtmlNode infoNode, string elemName)
        {
            var dataNode = infoNode?.SelectNodes($"./p[@class='{elemName}']/a");

            IDataBase<IName>[] data;

            if (dataNode?.Count > 0)
            {
                data = new DataBase<IName>[dataNode.Count];

                for (int i = 0; i < dataNode.Count; i++)
                {
                    Uri url = GetFullUrl(Decode(dataNode[i]?.Attributes["href"]?.Value));

                    string nameE = Decode(dataNode[i]?.Attributes["title"]?.Value);
                    IName nameData = new Name(english: nameE, url: url);

                    data[i] = new DataBase<IName>(nameData, url);
                }
            }
            else
            {
                data = Array.Empty<IDataBase<IName>>();
            }

            return data;
        }

        private ICollection<ICover> GetCovers(HtmlNode mainNode)
        {
            var imageNode = mainNode?.SelectSingleNode("./div[@class='detail-info-cover']/img");

            ICover[] images;

            if (imageNode != null)
            {
                string large = imageNode.Attributes["src"]?.Value;
                images = new ICover[] { new Cover(large, null, null) };
            }
            else
            {
                images = Array.Empty<ICover>();
            }

            return images;
        }

        private IDataBase<string> GetDescription(HtmlNode infoNode, Uri uri = default)
        {
            var descNode = infoNode?.SelectSingleNode("./p[@class='fullcontent']");

            if (descNode != null)
            {
                string description = Decode(descNode.InnerText);
                return new DataBase<string>(description, uri);
            }
            else
            {
                return null;
            }
        }

        private IName GetName(HtmlNode infoNode, Uri uri)
        {
            var nameNode = infoNode?.SelectSingleNode("./p[@class='detail-info-right-title']/span[@class='detail-info-right-title-font']");

            if (nameNode != null)
            {
                string nameE = nameNode.InnerText;
                return new Name(english: nameE, url: uri);
            }
            else
            {
                return new Name();
            }
        }

        #endregion Data getting methods

        #region Chapters methods

        private Uri PrepareChaptersUrl(Uri url)
        {
            if (!url.Host.Contains("m."))
            {
                UriBuilder uriBuilder = new UriBuilder(url);
                uriBuilder.Host = "m." + uriBuilder.Host;
                return uriBuilder.Uri;
            }

            return url;
        }

        private IEnumerable<IChapter> GetChapters(HtmlNode mainNode)
        {
            var chapters = mainNode?.SelectNodes(".//a");

            if (chapters?.Count > 0)
            {
                for (int i = chapters.Count - 1; i >= 0; i--)
                {
                    UriBuilder uriBuilder = new UriBuilder("https:" + chapters[i].Attributes["href"]?.Value)
                    {
                        Port = -1
                    };

                    // Removing 'm.' for a more consistent chapter url
                    uriBuilder.Host = uriBuilder.Host.Replace("m.", String.Empty);

                    Uri url = uriBuilder.Uri;

                    string nameE = Decode(chapters[i].InnerText);
                    IDataBase<string> nameDataE = new DataBase<string>(nameE, url);
                    IName nameData = new Name(english: nameDataE);

                    string dateString = Decode(chapters[i].SelectSingleNode("./span")?.InnerText);
                    DateTime.TryParse(dateString, out DateTime date);

                    IChapter chapter = new Chapter(nameData, url, date);

                    yield return chapter;
                }
            }
        }

        #endregion Chapters methods

        #region Pages methods

        private Uri PreparePagesUrl(Uri url)
        {
            UriBuilder uriBuilder = new UriBuilder(url);

            if (!url.Host.Contains("m."))
            {
                uriBuilder.Host = "m." + uriBuilder.Host;
            }

            uriBuilder.Path = uriBuilder.Path.Replace("manga", "roll_manga");
            return uriBuilder.Uri;
        }

        private IEnumerable<IDataBase> GetMangaPages(HtmlNode mainNode)
        {
            var images = mainNode.SelectNodes("./div[@class='mangaread-img']/img");

            foreach (var image in images)
            {
                string url = image.Attributes["data-original"]?.Value;

                yield return new DataBase(default, url);
            }
        }

        /// <summary>
        /// Pattern for page number search and replace
        /// </summary>
        private const string pattern = @"\d{3}";

        private readonly Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);

        /// <summary>
        /// Experimental method, maybe we can remove it if <see cref="GetMangaPages(HtmlNode)"/> is stable.
        /// </summary>
        /// <param name="mainNode"></param>
        /// <returns></returns>
        [Browsable(false)]
        private IEnumerable<IDataBase> GetMangaPagesExp(HtmlNode mainNode)
        {
            string imgNodePath = mainNode.XPath + "/div[@class='mangaread-img']/a/img";

            var Counters = mainNode.SelectNodes("./div[@class='mangaread-operate  clearfix']/select/option");

            if (Counters?.Count > 0)
            {
                Uri mainImg = new Uri(mainNode.SelectSingleNode(imgNodePath)?.Attributes["src"]?.Value);

                string path = mainImg.LocalPath;

                string dir = Path.GetDirectoryName(path);
                string name = Path.GetFileName(path);

                MatchCollection matchs = regex.Matches(name);
                var match = matchs.Last();
                var number = match.Value;

                // Trying to parse page number
                var startupValue = Int32.Parse(number);

                for (int i = startupValue; i < Counters.Count + startupValue; i++)
                {
                    // Check if this is a last page number, then we jusut load the last page (because the last page may have a non-numeric name)
                    if (i == Counters.Count + startupValue - 1)
                    {
                        var doc = Web.Load("https:" + Counters.Last().Attributes["value"].Value);

                        string img = doc.DocumentNode.SelectSingleNode(imgNodePath)?.Attributes["src"]?.Value;
                        yield return new DataBase(default, img);
                        continue;
                    }

                    // Calculate next page number
                    // I don't think there is a chapter with more than 999999 pages...
                    //int cLenght = i < 10 ? 1 : i < 100 ? 2 : i < 1000 ? 3 : i < 10000 ? 4 : i < 100000 ? 5 : 6;

                    // Replace page number
                    string newName = name.Remove(match.Index, match.Length).Insert(match.Index, i.ToString().PadLeft(match.Length, '0'));
                    string newPath = Path.Combine(dir, newName);

                    UriBuilder uriBuilder = new UriBuilder(mainImg)
                    {
                        Path = newPath,
                        Port = -1
                    };

                    yield return new DataBase(default, uriBuilder.Uri);
                }
            }
        }

        /// <summary>
        /// Old method, maybe we can remove it if <see cref="GetMangaPages(HtmlNode)"/> is stable.
        /// </summary>
        /// <param name="mainNode"></param>
        /// <returns></returns>
        [Obsolete("Slower, but stable")]
        private IEnumerable<IDataBase> GetMangaPagesOld(HtmlNode mainNode)
        {
            string firstImgNodePath = mainNode.XPath + "/div[@class='mangaread-img']/a/img";
            string secondtImgNodePath = mainNode.XPath + "/img";
            string img;

            img = mainNode.SelectSingleNode(firstImgNodePath)?.Attributes["src"]?.Value;
            yield return new DataBase(default, img);

            img = mainNode.SelectSingleNode(secondtImgNodePath)?.Attributes["src"]?.Value;

            if (img != null)
            {
                yield return new DataBase(default, img);
            }

            var Counters = mainNode.SelectNodes("./div[@class='mangaread-operate  clearfix']/select/option");

            if (Counters?.Count > 2)
            {
                for (int i = 2; i < Counters.Count; i += 2)
                {
                    var doc = Web.Load("https:" + Counters[i].Attributes["value"].Value);

                    img = doc.DocumentNode.SelectSingleNode(firstImgNodePath)?.Attributes["src"]?.Value;

                    if (img is null)
                    {
                        i--;
                        continue;
                    }

                    yield return new DataBase(default, img);

                    if (i < Counters.Count - 1)
                    {
                        img = doc.DocumentNode.SelectSingleNode(secondtImgNodePath)?.Attributes["src"]?.Value;
                        yield return new DataBase(default, img);
                    }
                }
            }
        }

        #endregion Pages methods

        #endregion Private Methods

        #endregion Methods
    }
}