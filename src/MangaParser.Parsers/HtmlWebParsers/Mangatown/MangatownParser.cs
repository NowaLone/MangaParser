using HtmlAgilityPack;
using MangaParser.Core.Exceptions;
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

        public override IEnumerable<IMangaObject> SearchManga(string query)
        {
            query = query is null ? String.Empty : query.Replace(' ', '+');

            var htmlDoc = Web.Load(BaseUrl + $"/search.php?name={query}");

            return SearchMangaCore(htmlDoc);
        }

        protected override IEnumerable<IMangaObject> SearchMangaCore(HtmlDocument htmlDoc)
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

        protected override IMangaObject GetMangaCore(HtmlDocument htmlDoc, Uri url)
        {
            if (htmlDoc is null)
            {
                throw new ArgumentNullException(nameof(htmlDoc));
            }

            var mainNode = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='article_content']");

            return GetMangaData(mainNode, url);
        }

        #endregion GetManga

        #region GetChapters

        protected override IEnumerable<IChapter> GetChaptersCore(HtmlDocument htmlDoc, Uri url)
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

            url = PreparePagesUri(url);

            var htmlDoc = Web.Load(url);

            return GetPagesCore(htmlDoc, url);
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

            url = PreparePagesUri(url);

            var htmlDoc = await Web.LoadFromWebAsync(url.OriginalString).ConfigureAwait(false);

            return GetPagesCore(htmlDoc, url);
        }

        protected override IEnumerable<IDataBase> GetPagesCore(HtmlDocument htmlDoc, Uri url)
        {
            if (htmlDoc is null)
            {
                throw new ArgumentNullException(nameof(htmlDoc));
            }

            var mainNode = htmlDoc.DocumentNode.SelectSingleNode("/html/body/div[@class='site-content']/section");

            return GetMangaPages(mainNode);
        }

        #endregion GetPages

        #region Private Methods

        #region Search methods

        private IEnumerable<IMangaObject> GetSearchResult(HtmlNode mainNode)
        {
            var thumbs = mainNode?.SelectNodes("./li");

            if (thumbs?.Count > 0)
            {
                for (int i = 0; i < thumbs.Count; i++)
                {
                    Uri url = GetFullUrl(thumbs[i].SelectSingleNode("./a")?.Attributes["href"]?.Value);

                    string nameE = thumbs[i].SelectSingleNode("./a")?.Attributes["title"]?.Value;
                    IDataBase<string> nameDataE = new DataBase<string>(nameE, url);
                    IName name = new Name(english: nameDataE);

                    ICollection<IDataBase<IName>> authors = GetThumbItems(thumbs[i], "view");
                    ICollection<IDataBase<IName>> genres = GetThumbItems(thumbs[i], "keyWord");

                    ICover cover = new Cover(thumbs[i].SelectSingleNode("./a/img")?.Attributes["src"]?.Value, null, null);
                    ICollection<ICover> covers = new ICover[] { cover };

                    var manga = new MangaObject(name, url)
                    {
                        Authors = authors,
                        Genres = genres,
                        Covers = covers,
                    };

                    yield return manga;
                }
            }
        }

        private ICollection<IDataBase<IName>> GetThumbItems(HtmlNode node, string key)
        {
            var items = node?.SelectNodes($"./p[@class='{key}']/a");

            IDataBase<IName>[] data;

            if (items?.Count > 0)
            {
                data = new IDataBase<IName>[items.Count];

                for (int i = 0; i < items.Count; i++)
                {
                    Uri url = base.GetFullUrl(items[i].Attributes["href"]?.Value);

                    string nameE = Decode(items[i].Attributes["title"] != null ? items[i].Attributes["title"].Value : items[i].InnerText);
                    IDataBase<string> nameDataE = new DataBase<string>(nameE, url);
                    IName nameData = new Name(english: nameDataE);

                    data[i] = new DataBase<IName>(nameData, url);
                }
            }
            else
            {
                data = Array.Empty<IDataBase<IName>>();
            }

            return data;
        }

        #endregion Search methods

        #region Data getting methods

        private IMangaObject GetMangaData(HtmlNode mainNode, Uri uri)
        {
            var infoNode = mainNode?.SelectSingleNode(".//div[@class='detail_info clearfix']/ul");

            IName name = GetName(mainNode, uri);

            IDataBase<string> description = GetDescription(mainNode, uri);
            ICollection<ICover> covers = GetCovers(mainNode);
            ICollection<IDataBase<IName>> genres = GetInfoData(infoNode, 4).Concat(GetInfoData(infoNode, 5)).ToList();
            ICollection<IDataBase<IName>> authors = GetInfoData(infoNode, 6);
            ICollection<IDataBase<IName>> illustrators = GetInfoData(infoNode, 7);

            var manga = new MangaObject(name, uri)
            {
                Description = description,
                Covers = covers,
                Genres = genres,
                Authors = authors,
                Illustrators = illustrators,
            };

            return manga;
        }

        private ICollection<IDataBase<IName>> GetInfoData(HtmlNode infoNode, int liNumber)
        {
            var dataNodes = infoNode?.SelectNodes($"./li[{liNumber}]/a");

            IDataBase<IName>[] data;

            if (dataNodes != null)
            {
                data = new DataBase<IName>[dataNodes.Count];

                for (int i = 0; i < dataNodes.Count; i++)
                {
                    Uri url = base.GetFullUrl(dataNodes[i].Attributes["href"]?.Value);

                    string nameE = Decode(dataNodes[i].Attributes["title"] != null ? dataNodes[i].Attributes["title"].Value : dataNodes[i].InnerText);
                    IDataBase<string> nameDataE = new DataBase<string>(nameE, url);
                    IName nameData = new Name(english: nameDataE);

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
            var imageNode = mainNode?.SelectSingleNode(".//div[@class='detail_info clearfix']/img");

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

        private IDataBase<string> GetDescription(HtmlNode mainNode, Uri uri)
        {
            var descNode = mainNode?.SelectSingleNode(".//span[@id='show']");

            if (descNode != null)
            {
                string description = Decode(descNode.InnerText);
                // Removing ' HIDE' word from description at the end
                description = description.Remove(description.Length - 5, 5);
                return new DataBase<string>(description, uri);
            }
            else
            {
                return null;
            }
        }

        private IName GetName(HtmlNode mainNode, Uri uri)
        {
            var engNameNode = mainNode?.SelectSingleNode("./h1");
            var orgNameNode = mainNode?.SelectSingleNode(".//div[@class='detail_info clearfix']/ul/li[3]/text()");

            if (orgNameNode != null || engNameNode != null)
            {
                string nameE = Decode(engNameNode.InnerText);
                string nameO = null;

                var orgNames = orgNameNode.InnerText.Split(';');
                if (orgNames.Length > 0)
                {
                    nameO = Decode(orgNames[0]);
                }

                return new Name(english: nameE, original: nameO, url: uri);
            }
            else
            {
                return new Name();
            }
        }

        #endregion Data getting methods

        #region Chapters methods

        private IEnumerable<IChapter> GetChapters(HtmlNode mainNode)
        {
            var chapters = mainNode?.SelectNodes("./ul[@class='chapter_list']/li");

            if (chapters?.Count > 0)
            {
                for (int i = chapters.Count - 1; i >= 0; i--)
                {
                    Uri url = GetFullUrl(chapters[i].SelectSingleNode("./a")?.Attributes["href"]?.Value);

                    string nameE = Decode(chapters[i].SelectSingleNode("./a")?.InnerText);

                    string additional = String.Empty;

                    var additionals = chapters[i].SelectNodes("./span[not(@class='time')]");

                    if (additionals != null)
                    {
                        foreach (HtmlNode node in additionals)
                        {
                            additional += " " + Decode(node.InnerText);
                        }
                    }

                    IDataBase<string> nameDataE = new DataBase<string>(nameE + additional, url);
                    IName nameData = new Name(english: nameDataE);

                    string dateString = Decode(chapters[i].SelectSingleNode("./span[@class='time']")?.InnerText);
                    DateTime.TryParse(dateString, out DateTime date);

                    IChapter chapter = new Chapter(nameData, url, date);

                    yield return chapter;
                }
            }
        }

        #endregion Chapters methods

        #region Pages methods

        private Uri PreparePagesUri(Uri uri)
        {
            UriBuilder uriBuilder = new UriBuilder(uri);

            uriBuilder.Host = uriBuilder.Host.Replace("www", "m");

            return uriBuilder.Uri;
        }

        private IEnumerable<IDataBase> GetMangaPages(HtmlNode mainNode)
        {
            string firstImgNodePath = mainNode.XPath + "/div[@class='mangaread-main']/div[@class='mangaread-img']/a/img";
            string secondtImgNodePath = mainNode.XPath + "/img";
            string img;

            img = mainNode.SelectSingleNode(firstImgNodePath)?.Attributes["src"]?.Value;
            yield return new DataBase(default, img);

            img = mainNode.SelectSingleNode(secondtImgNodePath)?.Attributes["src"]?.Value;
            if (img != null)
            {
                yield return new DataBase(default, img);
            }

            var Counters = mainNode.SelectNodes(".//div/div[2]/div/select/option[not(contains(@value,'featured'))]");

            if (Counters?.Count > 0)
            {
                for (int i = 2; i < Counters.Count; i += 2)
                {
                    var doc = Web.Load(Counters[i].Attributes["value"]?.Value);

                    img = doc.DocumentNode.SelectSingleNode(firstImgNodePath)?.Attributes["src"]?.Value;

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