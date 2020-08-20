using HtmlAgilityPack;
using MangaParser.Core.Interfaces;
using MangaParser.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;

namespace MangaParser.Parsers.HtmlWebParsers.Mangapanda
{
    public class MangapandaParser : CoreParser
    {
        #region Constructors

        public MangapandaParser(string baseUri = "http://www.mangapanda.com") : base(baseUri)
        {
        }

        #endregion Constructors

        #region Methods

        #region Search

        public override IEnumerable<IMangaObject> SearchManga(string query)
        {
            query = query is null ? String.Empty : query.Replace(' ', '+');

            var htmlDoc = Web.Load(BaseUrl + $"search/?w={query}");

            return SearchMangaCore(htmlDoc);
        }

        protected override IEnumerable<IMangaObject> SearchMangaCore(HtmlDocument htmlDoc)
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

        protected override IMangaObject GetMangaCore(HtmlDocument htmlDoc, Uri url)
        {
            if (htmlDoc is null)
            {
                throw new ArgumentNullException(nameof(htmlDoc));
            }

            var mainNode = htmlDoc.DocumentNode.SelectSingleNode("//div[@id='wrapper_body']");

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

            var mainNode = htmlDoc.DocumentNode.SelectSingleNode("//div[@id='wrapper_body']");

            return GetChapters(mainNode);
        }

        #endregion GetChapters

        #region GetPages

        protected override IEnumerable<IDataBase> GetPagesCore(HtmlDocument htmlDoc, Uri url)
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

        #region Search methods

        private IEnumerable<MangaObject> GetSearchResult(HtmlNode mainNode)
        {
            var thumbs = mainNode?.SelectNodes("//div[@class='mangaresultinner']");

            if (thumbs?.Count > 0)
            {
                for (int i = 0; i < thumbs.Count; i++)
                {
                    Uri url = GetFullUrl(thumbs[i].SelectSingleNode("./div[@class='result_info c5']/div[@class='manga_name']/div/h3/a")?.Attributes["href"]?.Value);

                    string nameE = Decode(thumbs[i].SelectSingleNode("./div[@class='result_info c5']/div[@class='manga_name']/div/h3/a")?.InnerText);
                    IDataBase<string> nameDataE = new DataBase<string>(nameE, url);
                    IName name = new Name(english: nameDataE);

                    ICollection<IDataBase<IName>> genres = GetThumbGenres(thumbs[i].SelectSingleNode("./div[@class='result_info c5']/div[@class='manga_genre']"));

                    string coverstr = thumbs[i].SelectSingleNode("./div[@class='imgsearchresults']")?.Attributes["style"]?.Value;
                    string coverS = coverstr.Substring(coverstr.IndexOf("('") + 2, coverstr.IndexOf("')") - coverstr.IndexOf("('") - 2);
                    Uri[] fullCovers = GetFullCovers(coverS);
                    ICover cover = new Cover(fullCovers[0], fullCovers[1], fullCovers[2]);
                    ICollection<ICover> covers = new ICover[] { cover };

                    var manga = new MangaObject(name, url)
                    {
                        Genres = genres,
                        Covers = covers,
                    };

                    yield return manga;
                }
            }
        }

        private ICollection<IDataBase<IName>> GetThumbGenres(HtmlNode node)
        {
            var items = node?.InnerText.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);

            IDataBase<IName>[] genres;

            if (items?.Length > 0)
            {
                genres = new IDataBase<IName>[items.Length];

                for (int i = 0; i < items.Length; i++)
                {
                    string nameE = Decode(items[i]);

                    Uri url = base.GetFullUrl(Path.Combine("popular", nameE.Replace(' ', '-').ToLower()));

                    IDataBase<string> nameDataE = new DataBase<string>(nameE, url);
                    IName nameData = new Name(english: nameDataE);

                    genres[i] = new DataBase<IName>(nameData, url);
                }
            }
            else
            {
                genres = Array.Empty<IDataBase<IName>>();
            }

            return genres;
        }

        #endregion Search methods

        #region Data getting methods

        private MangaObject GetMangaData(HtmlNode mainNode, Uri uri)
        {
            var infoNode = mainNode?.SelectSingleNode("//div[@id='mangaproperties']/table");

            IName name = GetName(infoNode, uri);

            IDataBase<string> description = GetDescription(mainNode, uri);
            ICollection<ICover> covers = GetCovers(mainNode);
            ICollection<IDataBase<IName>> genres = GetGenres(infoNode);
            ICollection<IDataBase<IName>> authors = GetInfoData<IName>(infoNode, 5);
            ICollection<IDataBase<IName>> illustrators = GetInfoData<IName>(infoNode, 6);
            IDataBase<DateTime> releasedate = GetInfoData<DateTime>(infoNode, 3).FirstOrDefault();

            var manga = new MangaObject(name, uri)
            {
                Description = description,
                Covers = covers,
                Genres = genres,
                Authors = authors,
                Illustrators = illustrators,
                ReleaseDate = releasedate
            };

            return manga;
        }

        private ICollection<IDataBase<T>> GetInfoData<T>(HtmlNode infoNode, int trNumber)
        {
            var dataNode = infoNode?.SelectSingleNode($"./tr[{trNumber}]/td[2]");

            IDataBase<T>[] data = Array.Empty<DataBase<T>>();

            if (dataNode != null)
            {
                string value = Decode(dataNode.InnerText);

                if (typeof(T) == typeof(IName))
                {
                    var names = value.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);

                    if (names.Length > 0)
                    {
                        data = new IDataBase<T>[names.Length];

                        for (int i = 0; i < names.Length; i++)
                        {
                            string nameE = names[i];

                            IDataBase<string> nameDataE = new DataBase<string>(nameE, default(Uri));

                            IName nameData = new Name(english: nameDataE);

                            data[i] = new DataBase<T>((T)nameData, default(Uri));
                        }
                    }
                }
                else if (typeof(T) == typeof(DateTime))
                {
                    data = new IDataBase<T>[1];

                    if (DateTime.TryParseExact(value, "yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var result))
                    {
                        // Oh no! boxing! What to do? Nothing!
                        data[0] = new DataBase<T>((T)(object)result, default(Uri));
                    }
                    else
                    {
                        data[0] = new DataBase<T>(default, default(Uri));
                    }
                }
            }

            return data;
        }

        private ICollection<ICover> GetCovers(HtmlNode mainNode)
        {
            var imageNode = mainNode?.SelectSingleNode("//img");

            ICover[] images;

            if (imageNode != null)
            {
                string coverL = imageNode.Attributes["src"]?.Value;
                Uri[] fullCovers = GetFullCovers(coverL);

                ICover cover = new Cover(fullCovers[0], fullCovers[1], fullCovers[2]);

                images = new ICover[] { cover };
            }
            else
            {
                images = Array.Empty<ICover>();
            }

            return images;
        }

        private IDataBase<string> GetDescription(HtmlNode mainNode, Uri uri)
        {
            var descNode = mainNode?.SelectSingleNode("./div[@id='readmangasum']/p");

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

        private ICollection<IDataBase<IName>> GetGenres(HtmlNode infoNode)
        {
            var genreNodes = infoNode?.SelectNodes("./tr[8]//a");

            IDataBase<IName>[] genres;

            if (genreNodes?.Count > 0)
            {
                genres = new IDataBase<IName>[genreNodes.Count];

                for (int i = 0; i < genreNodes.Count; i++)
                {
                    Uri url = base.GetFullUrl(genreNodes[i].Attributes["href"]?.Value);

                    string nameE = Decode(genreNodes[i].SelectSingleNode("./span")?.InnerText);
                    IDataBase<string> nameDataE = new DataBase<string>(nameE, url);
                    IName nameData = new Name(english: nameDataE);

                    genres[i] = new DataBase<IName>(nameData, url);
                }
            }
            else
            {
                genres = Array.Empty<IDataBase<IName>>();
            }

            return genres;
        }

        private IName GetName(HtmlNode infoNode, Uri uri)
        {
            var engNameNode = infoNode?.SelectSingleNode("./tr[1]/td[2]/h2[@class='aname']");
            var orgNameNode = infoNode?.SelectSingleNode("./tr[2]/td[2]");

            if (orgNameNode != null || engNameNode != null)
            {
                string nameE = Decode(engNameNode.InnerText);
                string nameO = null;

                if (!String.IsNullOrWhiteSpace(orgNameNode.InnerText))
                {
                    var orgNames = orgNameNode.InnerText.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
                    if (orgNames.Length > 0)
                    {
                        nameO = Decode(orgNames[0]);
                    }
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
            var chapters = mainNode?.SelectNodes("//div[@id='chapterlist']/table/tr[not(@class='table_head')]");

            if (chapters?.Count > 0)
            {
                for (int i = 0; i < chapters.Count; i++)
                {
                    Uri url = GetFullUrl(chapters[i].SelectSingleNode("./td/a")?.Attributes["href"]?.Value);

                    var nameE = Decode(chapters[i].SelectSingleNode("./td/a")?.InnerText);
                    var additional = Decode(chapters[i].SelectSingleNode("./td/text()[3]")?.InnerText);
                    additional = additional == ":" ? String.Empty : additional;
                    IDataBase<string> nameDataE = new DataBase<string>(nameE + additional, url);
                    IName nameData = new Name(english: nameDataE);

                    string dateString = Decode(chapters[i].SelectSingleNode("./td[2]")?.InnerText);
                    DateTime.TryParseExact(dateString, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime date);

                    IChapter chapter = new Chapter(nameData, url, date);

                    yield return chapter;
                }
            }
        }

        #endregion Chapters methods

        #region Pages methods

        private IEnumerable<IDataBase> GetMangaPages(HtmlNode mainNode)
        {
            string imgNodePath = mainNode?.XPath + "//img[@id='img']";
            string img;

            img = mainNode.SelectSingleNode(imgNodePath)?.Attributes["src"]?.Value;
            yield return new DataBase(default, img);

            var Counters = mainNode.SelectNodes("./div[@id='topchapter']/div[@id='navi']/div[@id='selectpage']/select/option");

            if (Counters?.Count > 1)
            {
                using (HttpClient client = new HttpClient())
                {
                    for (int i = 1; i < Counters.Count; i++)
                    {
                        var doc = Web.Load(GetFullUrl(Counters[i].Attributes["value"]?.Value));

                        img = doc.DocumentNode.SelectSingleNode(imgNodePath)?.Attributes["src"]?.Value;

                        yield return new DataBase(default, img);
                    }
                }
            }
        }

        #endregion Pages methods

        /// <summary>
        /// Returns all cover sizes (large, medium, small).
        /// </summary>
        /// <param name="cover">Known cover size url.</param>
        /// <returns>All cover sizes (large, medium, small).</returns>
        private Uri[] GetFullCovers(string cover)
        {
            Uri coverL = null, coverM = null, coverS = null;
            UriBuilder uriBuilder;

            Uri coverUri = new Uri(cover);

            string dir = Path.GetDirectoryName(coverUri.LocalPath);
            string ext = Path.GetExtension(cover);

            string coverFileName = Path.GetFileNameWithoutExtension(cover);

            switch (coverFileName.Substring(coverFileName.Length - 2, 2))
            {
                case "r0":
                    {
                        string coverLFileName = coverFileName.Remove(coverFileName.Length - 2, 2) + "l0";
                        string coverMFileName = coverFileName.Remove(coverFileName.Length - 2, 2) + "m0";

                        uriBuilder = new UriBuilder(cover)
                        {
                            Path = Path.Combine(dir, coverLFileName + ext),
                            Port = -1
                        };
                        coverL = uriBuilder.Uri;

                        uriBuilder = new UriBuilder(cover)
                        {
                            Path = Path.Combine(dir, coverMFileName + ext),
                            Port = -1
                        };
                        coverM = uriBuilder.Uri;

                        coverS = new Uri(cover);
                    };
                    break;

                case "m0":
                    {
                        string coverLFileName = coverFileName.Remove(coverFileName.Length - 2, 2) + "l0";
                        string coverSFileName = coverFileName.Remove(coverFileName.Length - 2, 2) + "r0";

                        uriBuilder = new UriBuilder(cover)
                        {
                            Path = Path.Combine(dir, coverLFileName + ext),
                            Port = -1
                        };
                        coverL = uriBuilder.Uri;

                        coverM = new Uri(cover);

                        uriBuilder = new UriBuilder(cover)
                        {
                            Path = Path.Combine(dir, coverSFileName + ext),
                            Port = -1
                        };
                        coverS = uriBuilder.Uri;
                    }
                    break;

                case "l0":
                    {
                        string coverMFileName = coverFileName.Remove(coverFileName.Length - 2, 2) + "m0";
                        string coverSFileName = coverFileName.Remove(coverFileName.Length - 2, 2) + "r0";

                        coverL = new Uri(cover);

                        uriBuilder = new UriBuilder(cover)
                        {
                            Path = Path.Combine(dir, coverMFileName + ext),
                            Port = -1
                        };
                        coverM = uriBuilder.Uri;

                        uriBuilder = new UriBuilder(cover)
                        {
                            Path = Path.Combine(dir, coverSFileName + ext),
                            Port = -1
                        };
                        coverS = uriBuilder.Uri;
                    }
                    break;

                default:
                    break;
            }

            return new Uri[3] { coverL, coverM, coverS };
        }

        #endregion Private Methods

        #endregion Methods
    }
}