using HtmlAgilityPack;
using MangaParser.Core.Interfaces;
using MangaParser.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace MangaParser.Parsers.HtmlWebParsers.ReadManga
{
    public class ReadMangaParser : CoreParser
    {
        #region Fields

        /// <summary>
        /// Js array name.
        /// </summary>
        private const string jsArrayVar = "rm_h.init";

        /// <summary>
        /// Pattern for js array scraping.
        /// </summary>
        private const string pattern = "\\[.*?'(http.*?)'\\,'(http.*?)'\\,\\\"(.*?)\\\"";

        private readonly Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);

        #endregion Fields

        #region Constructors

        public ReadMangaParser(string baseUrl = "https://readmanga.live") : base(baseUrl)
        {
        }

        #endregion Constructors

        #region Methods

        #region Search

        protected override IEnumerable<IMangaObject> SearchMangaCore(HtmlDocument htmlDoc)
        {
            if (htmlDoc is null)
            {
                throw new ArgumentNullException(nameof(htmlDoc));
            }

            var mainNode = htmlDoc.DocumentNode.SelectSingleNode("//body/div[@class='pageBlock container']/div[@class='leftContent']/div[@id='mangaResults']");

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

            var mainNode = htmlDoc.DocumentNode.SelectSingleNode("//body/div[@class='pageBlock container']/div[@class='leftContent']");

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

            var mainNode = htmlDoc.DocumentNode.SelectSingleNode("//body/div[@class='pageBlock container']/div[@class='leftContent']");

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

        private IEnumerable<IMangaObject> GetSearchResult(HtmlNode mainNode)
        {
            var thumbs = mainNode?.SelectNodes("./div[@class='tiles row']/div[@class='tile col-sm-6 ']");

            if (thumbs?.Count > 0)
            {
                for (int i = 0; i < thumbs.Count; i++)
                {

                    Uri url = GetFullUrl(thumbs[i].SelectSingleNode("./div[@class='desc']/h3/a")?.Attributes["href"]?.Value);

                    string description = GetThumbDescription(thumbs[i]);

                    string nameL = Decode(thumbs[i].SelectSingleNode("./div[@class='desc']/h3/a")?.InnerText);
                    string nameE = Decode(thumbs[i].SelectSingleNode("./div[@class='desc']/h4")?.InnerText);
                    IDataBase<string> nameDataL = new DataBase<string>(nameL, url);
                    IDataBase<string> nameDataE = !String.IsNullOrWhiteSpace(nameE) ? new DataBase<string>(nameE, url) : null;
                    IName name = new Name(nameDataL, nameDataE);

                    ICollection<IDataBase<IName>> authors = GetThumbItems(thumbs[i], "person");
                    ICollection<IDataBase<IName>> genres = GetThumbItems(thumbs[i], "element");

                    ICover cover = new Cover(small: thumbs[i].SelectSingleNode(".//img")?.Attributes["data-original"]?.Value);

                    ICollection<ICover> covers = new ICover[] { cover };

                    IDataBase<string> descriptionData = new DataBase<string>(description, url);

                    var manga = new MangaObject(name, url)
                    {
                        Authors = authors,
                        Genres = genres,
                        Covers = covers,
                        Description = descriptionData
                    };

                    yield return manga;
                }
            }
        }

        private ICollection<IDataBase<IName>> GetThumbItems(HtmlNode tileNode, string key)
        {
            var items = tileNode?.SelectNodes($".//a[@class='{key}-link']");

            IDataBase<IName>[] data;

            if (items?.Count > 0)
            {
                data = new IDataBase<IName>[items.Count];

                for (int i = 0; i < items.Count; i++)
                {
                    Uri url = base.GetFullUrl(items[i].Attributes["href"]?.Value);

                    string nameL = base.Decode(items[i].InnerText);
                    string nameE = Path.GetFileName(url.AbsoluteUri);

                    IDataBase<string> nameDataL = new DataBase<string>(nameL, url);
                    IDataBase<string> nameDataE = new DataBase<string>(nameE, url);

                    IName nameData = new Name(nameDataL, nameDataE);

                    data[i] = new DataBase<IName>(nameData, url);
                }
            }
            else
            {
                data = Array.Empty<IDataBase<IName>>();
            }

            return data;
        }

        private string GetThumbDescription(HtmlNode tileNode)
        {
            HtmlNode descriptionNode = tileNode.SelectSingleNode("./div[@class='desc']/div[@class='hidden long-description-holder']");

            if (descriptionNode is null)
            {
                return String.Empty;
            }

            HtmlNode excludedTitleNode = tileNode.SelectSingleNode("./div[@class='desc']/div[@class='hidden long-description-holder']/h5");

            if (excludedTitleNode is null)
            {
                return Decode(descriptionNode.InnerText);
            }
            else
            {
                descriptionNode.RemoveChild(excludedTitleNode);
                return Decode(descriptionNode.InnerText);
            }
        }

        #endregion Search methods

        #region Chapters methods

        private IEnumerable<IChapter> GetChapters(HtmlNode mainNode)
        {
            var chapters = mainNode?.SelectNodes("./div[contains(@class, 'chapters-link')]/table/tr");

            if (chapters?.Count > 0)
            {
                for (int i = chapters.Count - 1; i >= 0; i--)
                {
                    string nameL = Decode(chapters[i].SelectSingleNode("./td/a/text()")?.InnerText);

                    Uri url = GetFullUrl(chapters[i].SelectSingleNode("./td/a")?.Attributes["href"]?.Value, "mtr=1");

                    IDataBase<string> nameDataL = new DataBase<string>(nameL, url);

                    IName nameData = new Name(nameDataL);

                    string dateString = Decode(chapters[i].SelectSingleNode("./td[2]")?.InnerText);
                    DateTime.TryParseExact(dateString, "dd.MM.yy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime date);

                    IChapter chapter = new Chapter(nameData, url, date);

                    yield return chapter;
                }
            }
        }

        #endregion Chapters methods

        #region Data getting methods

        private IMangaObject GetMangaData(HtmlNode mainNode, Uri uri)
        {
            var infoNode = mainNode?.SelectSingleNode(".//div[@class='flex-row']/div[@class='subject-meta col-sm-7']");

            IName name = GetName(mainNode, uri);

            IDataBase<string> description = GetDescription(mainNode, uri);
            IDataBase<int> volumes = GetVolumes(infoNode);
            ICollection<ICover> covers = GetCovers(mainNode);
            ICollection<IDataBase<IName>> genres = GetInfoData<IName>(infoNode, "elem_genre ");
            ICollection<IDataBase<IName>> authors = GetInfoData<IName>(infoNode, "elem_author ");
            ICollection<IDataBase<IName>> writers = GetInfoData<IName>(infoNode, "elem_screenwriter ");
            ICollection<IDataBase<IName>> illustrators = GetInfoData<IName>(infoNode, "elem_illustrator ");
            ICollection<IDataBase<IName>> publishers = GetInfoData<IName>(infoNode, "elem_publisher ");
            ICollection<IDataBase<IName>> magazines = GetInfoData<IName>(infoNode, "elem_magazine ");
            IDataBase<DateTime> releasedate = GetInfoData<DateTime>(infoNode, "elem_year ").FirstOrDefault();

            var manga = new MangaObject(name, uri)
            {
                Description = description,
                Volumes = volumes,
                Covers = covers,
                Genres = genres,
                Authors = authors,
                Writers = writers,
                Illustrators = illustrators,
                Publishers = publishers,
                Magazines = magazines,
                ReleaseDate = releasedate,
            };

            return manga;
        }

        private ICollection<ICover> GetCovers(HtmlNode mainNode)
        {
            var data = mainNode?.SelectNodes(".//div[@class='flex-row']/div[@class='subject-cower col-sm-5']//img | .//div[@class='flex-row']/div[@class='subject-cower col-sm-5']//a");

            ICover[] covers;

            if (data?.Count > 0)
            {
                covers = new ICover[data.Count];

                for (int i = 0; i < data.Count; i++)
                {
                    string large = data[i].Attributes["data-full"]?.Value;
                    string medium = data[i].Attributes["src"]?.Value ?? data[i].Attributes["href"]?.Value;
                    string small = data[i].Attributes["data-thumb"]?.Value;

                    covers[i] = new Cover(large, medium, small);
                }
            }
            else
            {
                covers = Array.Empty<ICover>();
            }

            return covers;
        }

        private IDataBase<string> GetDescription(HtmlNode mainNode, Uri uri = default)
        {
            var descNode = mainNode?.SelectSingleNode("./meta[@itemprop='description']");

            if (descNode != null)
            {
                string description = Decode(descNode.Attributes["content"]?.Value);
                return new DataBase<string>(description, uri);
            }
            else
            {
                return null;
            }
        }

        private ICollection<IDataBase<T>> GetInfoData<T>(HtmlNode infoNode, string elemName)
        {
            var dataNode = infoNode?.SelectNodes($".//span[contains(@class, '{elemName}')]");

            IDataBase<T>[] data;

            if (dataNode?.Count > 0)
            {
                data = new DataBase<T>[dataNode.Count];

                for (int i = 0; i < dataNode.Count; i++)
                {
                    if (elemName == "elem_publisher ")
                    {
                        string nameE = Decode(dataNode[i].InnerText);

                        // Removing that strange separator
                        nameE = nameE.Replace(", ", String.Empty);

                        IDataBase<string> nameDataE = new DataBase<string>(nameE, default(Uri));

                        IName name = new Name(english: nameDataE);

                        data[i] = new DataBase<T>((T)name, default(Uri));
                    }
                    else
                    {
                        Uri url = GetFullUrl(dataNode[i].SelectSingleNode("./a")?.Attributes["href"]?.Value);

                        string value = Decode(dataNode[i].SelectSingleNode("./a")?.InnerText);

                        if (typeof(T) == typeof(IName))
                        {
                            string nameL = value;
                            string nameE = Path.GetFileName(url.AbsoluteUri);

                            IDataBase<string> nameDataL = new DataBase<string>(nameL, url);
                            IDataBase<string> nameDataE = new DataBase<string>(nameE, url);

                            // Skip english-only magazine names
                            IName nameData = elemName == "elem_magazine " ? new Name(english: nameDataL) : new Name(nameDataL, nameDataE);

                            data[i] = new DataBase<T>((T)nameData, url);
                        }
                        else if (typeof(T) == typeof(DateTime))
                        {
                            if (DateTime.TryParseExact(value, "yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var result))
                            {
                                // Oh no! boxing! What to do? Nothing!
                                data[i] = new DataBase<T>((T)(object)result, url);
                            }
                            else
                            {
                                data[i] = new DataBase<T>(default, url);
                            }
                        }
                        else
                        {
                            data[i] = new DataBase<T>((T)(object)value, url);
                        }
                    }
                }
            }
            else
            {
                data = Array.Empty<DataBase<T>>();
            }

            return data;
        }

        private IName GetName(HtmlNode mainNode, Uri uri = default)
        {
            var namesNodes = mainNode?.SelectNodes("./h1[@class='names']/span");

            IDataBase<string> local = default, eng = default, orig = default;

            if (namesNodes?.Count > 0)
            {
                foreach (HtmlNode node in namesNodes)
                {
                    switch (node.Attributes["class"]?.Value)
                    {
                        case "name":
                            {
                                string nameL = Decode(node.InnerText);
                                local = new DataBase<string>(nameL, uri);
                            }
                            break;

                        case "eng-name":
                            {
                                string nameE = Decode(node.InnerText);
                                eng = new DataBase<string>(nameE, uri);
                            }
                            break;

                        case "original-name":
                            {
                                string nameO = Decode(node.InnerText);
                                orig = new DataBase<string>(nameO, uri);
                            }
                            break;
                    }
                }
            }

            return new Name(local, eng, orig);
        }

        private IDataBase<int> GetVolumes(HtmlNode infoNode)
        {
            var volumesNode = infoNode?.SelectSingleNode("./p/text()[2]");

            if (volumesNode != null)
            {
                var volumes = Decode(volumesNode.InnerText);

                // Search ',' index if there is another text
                var index = volumes.IndexOf(',');
                string count = index > 0 ? volumes.Substring(0, index) : volumes;

                if (Int32.TryParse(count, out int result))
                {
                    return new DataBase<int>(result, default(Uri));
                }
            }

            return null;
        }

        #endregion Data getting methods

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
                            string url = matchs[i].Groups[1].Value + matchs[i].Groups[3].Value;

                            yield return new DataBase(default, url);
                        }
                    }
                }
            }
        }

        #endregion Pages methods

        #endregion Private Methods

        protected virtual Uri GetFullUrl(string path, string query)
        {
            UriBuilder uriBuilder = new UriBuilder(base.GetFullUrl(path))
            {
                Query = query,
            };

            return uriBuilder.Uri;
        }

        #endregion Methods
    }
}