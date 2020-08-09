using HtmlAgilityPack;
using MangaParser.Core.Interfaces;
using MangaParser.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace MangaParser.Parsers.HtmlWebParsers.Mangareader
{
    public class MangareaderParser : CoreParser
    {
        #region Fields

        /// <summary>
        /// Js array name.
        /// </summary>
        private const string jsArrayVar = "\"im\":";

        /// <summary>
        /// Pattern for js array scraping.
        /// </summary>
        private const string pattern = "(\"u\":)\\\"(.*?)\\\"";

        private readonly Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);

        #endregion Fields

        #region Constructors

        public MangareaderParser(string baseUri = "https://www.mangareader.net") : base(baseUri)
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

            var mainNode = htmlDoc.DocumentNode.SelectSingleNode("//div[@id='ares']");

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

            var mainNode = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='d14']/div[@class='d36']");

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

            var mainNode = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='d14']/div[@class='d36']/table[@class='d48']");

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

            string scriptText = htmlDoc.DocumentNode?.Descendants()?.Where(n => n.Name == "script" && n.InnerText.Contains(jsArrayVar)).FirstOrDefault()?.InnerText;

            return GetMangaPages(scriptText);
        }

        #endregion GetPages

        #region Private Methods

        #region Search methods

        private IEnumerable<MangaObject> GetSearchResult(HtmlNode mainNode)
        {
            var thumbs = mainNode?.SelectNodes("//div[@class='d54']");

            if (thumbs?.Count > 0)
            {
                for (int i = 0; i < thumbs.Count; i++)
                {
                    Uri url = GetFullUrl(thumbs[i].SelectSingleNode(".//div[@class='d57']/a")?.Attributes["href"]?.Value);

                    string nameE = Decode(thumbs[i].SelectSingleNode(".//div[@class='d57']/a")?.InnerText);
                    IDataBase<string> nameDataE = new DataBase<string>(nameE, url);
                    IName name = new Name(english: nameDataE);

                    ICollection<IDataBase<IName>> genres = GetThumbGenres(thumbs[i].SelectSingleNode(".//div[@class='d60']"));

                    string coverstr = "https:" + thumbs[i].SelectSingleNode(".//div[@class='d56 dlz']")?.Attributes["data-src"]?.Value;
                    Uri[] fullCovers = GetFullCovers(coverstr);
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
            var items = node?.InnerText.Split(", ");

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
            var infoNode = mainNode?.SelectSingleNode("./div[@class='d37']/div[@class='d39']/table");

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

            IDataBase<T>[] data;

            if (dataNode != null)
            {
                data = new IDataBase<T>[1];

                string value = Decode(dataNode.InnerText);

                if (typeof(T) == typeof(IName))
                {
                    string nameE = value;

                    IDataBase<string> nameDataE = new DataBase<string>(nameE, default(Uri));

                    IName nameData = new Name(english: nameDataE);

                    data[0] = new DataBase<T>((T)nameData, default(Uri));
                }
                else if (typeof(T) == typeof(DateTime))
                {
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
            else
            {
                data = Array.Empty<DataBase<T>>();
            }

            return data;
        }

        private ICollection<ICover> GetCovers(HtmlNode mainNode)
        {
            var imageNode = mainNode?.SelectSingleNode(".//img");

            ICover[] images;

            if (imageNode != null)
            {
                string coverL = imageNode.Attributes["src"]?.Value;
                if (!coverL.StartsWith("http:") && !coverL.StartsWith("https:"))
                {
                    coverL = "https:" + coverL;
                }

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
            var descNode = mainNode?.SelectSingleNode("./div[@class='d46']/p");

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

                    string nameE = Decode(genreNodes[i].InnerText);
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
            var engNameNode = infoNode?.SelectSingleNode("./tr[1]/td[2]/span[@class='name']");
            var orgNameNode = infoNode?.SelectSingleNode("./tr[2]/td[2]");

            if (orgNameNode != null || engNameNode != null)
            {
                string nameE = Decode(engNameNode.InnerText);
                string nameO = null;

                if (!String.IsNullOrWhiteSpace(orgNameNode.InnerText))
                {
                    var orgNames = orgNameNode.InnerText.Split(", ");
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
            var chapters = mainNode?.SelectNodes(".//tr[not(@class='d49')]");

            if (chapters?.Count > 0)
            {
                for (int i = 0; i < chapters.Count; i++)
                {
                    Uri url = GetFullUrl(chapters[i].SelectSingleNode("./td/a")?.Attributes["href"]?.Value);

                    var nameE = Decode(chapters[i].SelectSingleNode("./td/a")?.InnerText);
                    var additional = Decode(chapters[i].SelectSingleNode("./td/text()[2]")?.InnerText);
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

        private IEnumerable<IDataBase> GetMangaPages(string scriptText)
        {
            int index = scriptText.IndexOf(jsArrayVar);
            int arrayPosition = index + jsArrayVar.Length;

            if (index != -1 && (arrayPosition < scriptText.Length))
            {
                string array = scriptText.Substring(arrayPosition);

                if (array.Length > 0)
                {
                    MatchCollection matchs = regex.Matches(array);

                    if (matchs.Count > 0)
                    {
                        for (int i = 0; i < matchs.Count; i++)
                        {
                            string url = "https:" + matchs[i].Groups[2].Value.Replace("\\/", "/");

                            yield return new DataBase(default, url);
                        }
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