using HtmlAgilityPack;
using MangaParser.Core.Interfaces;
using MangaParser.Core.Models;
using System;
using System.Collections.Generic;
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

            var scriptText = htmlDoc.DocumentNode.Descendants()?.Where(n => n.Name == "script" && n.InnerText.Contains(jsArrayVar)).First().InnerText;

            return GetMangaPages(scriptText);
        }

        #endregion GetPages

        #region Private Methods

        #region Search methods

        private IEnumerable<IMangaObject> GetSearchResult(HtmlNode mainNode)
        {
            var thumbs = mainNode?.SelectNodes("./div[@class='tiles row']/div[@class='tile col-sm-6 ']");

            if (thumbs != null)
            {
                for (int i = 0; i < thumbs.Count; i++)
                {
                    string nameL = Decode(thumbs[i].SelectSingleNode("./div[@class='desc']/h3/a")?.InnerText);
                    string nameE = base.Decode(thumbs[i].SelectSingleNode("./div[@class='desc']/h4")?.InnerText);

                    Uri url = new Uri(BaseUrl, thumbs[i].SelectSingleNode("./div[@class='desc']/h3/a")?.Attributes["href"]?.Value);

                    IDataBase<string> nameDataL = new DataBase<string>(nameL, url);
                    IDataBase<string> nameDataE = new DataBase<string>(nameE, url);

                    IName name = new Name(nameDataL, nameDataE);

                    IDataBase<IName>[] authors = GetThumbAuthors(thumbs[i]);
                    IDataBase<IName>[] genres = GetThumbGenres(thumbs[i]);

                    ICover[] covers = new Cover[] { new Cover(small: thumbs[i].SelectSingleNode(".//img")?.Attributes["data-original"]?.Value) };

                    var manga = new MangaObject(name, url)
                    {
                        Authors = authors,
                        Genres = genres,
                        Covers = covers
                    };

                    yield return manga;
                }
            }
        }

        private IDataBase<IName>[] GetThumbAuthors(HtmlNode tileNode)
        {
            var authors = tileNode?.SelectNodes(".//a[@class='person-link']");

            DataBase<IName>[] Authors;

            if (authors?.Count > 0)
            {
                Authors = new DataBase<IName>[authors.Count];

                for (int i = 0; i < authors.Count; i++)
                {
                    string url = BaseUrl + authors[i].Attributes["href"]?.Value;

                    string nameL = base.Decode(authors[i].InnerText);
                    string nameE = System.IO.Path.GetFileName(url);

                    Name nameData = new Name(new DataBase<string>(nameL, url), new DataBase<string>(nameE, url));

                    Authors[i] = new DataBase<IName>(nameData, url);
                }
            }
            else
            {
                Authors = Array.Empty<DataBase<IName>>();
            }

            return Authors;
        }

        private IDataBase<IName>[] GetThumbGenres(HtmlNode tileNode)
        {
            var genres = tileNode?.SelectNodes(".//a[@class='element-link']");

            DataBase<IName>[] Genres;

            if (genres?.Count > 0)
            {
                Genres = new DataBase<IName>[genres.Count];

                for (int i = 0; i < genres.Count; i++)
                {
                    string url = BaseUrl + genres[i].Attributes["href"]?.Value;

                    string nameL = Decode(genres[i].InnerText);
                    string nameE = System.IO.Path.GetFileName(url);

                    Name nameData = new Name(new DataBase<string>(nameL, url), new DataBase<string>(nameE, url));

                    Genres[i] = new DataBase<IName>(nameData, url);
                }
            }
            else
            {
                Genres = Array.Empty<DataBase<IName>>();
            }

            return Genres;
        }

        #endregion Search methods

        #region Data getting methods

        private IChapter[] GetChapters(HtmlNode mainNode)
        {
            var chapters = mainNode?.SelectNodes("./div[contains(@class, 'chapters-link')]/table/tr");

            Chapter[] Chapters;

            if (chapters?.Count > 0)
            {
                Chapters = new Chapter[chapters.Count];

                for (int i = chapters.Count - 1; i >= 0; i--)
                {
                    DateTime.TryParseExact(Decode(chapters[i].SelectSingleNode("./td[2]")?.InnerText), "dd.MM.yy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime date);

                    string name = Decode(chapters[i].SelectSingleNode("./td/a")?.InnerText);
                    string url = BaseUrl + chapters[i].SelectSingleNode("./td/a")?.Attributes["href"]?.Value + "?mtr=1";

                    Name nameData = new Name(new DataBase<string>(name, url));

                    Chapters[chapters.Count - 1 - i] = new Chapter(nameData, url, date);
                }
            }
            else
            {
                Chapters = Array.Empty<Chapter>();
            }

            return Chapters;
        }

        private ICover[] GetCovers(HtmlNode mainNode)
        {
            var imagesNode = mainNode?.SelectSingleNode(".//div[@class='flex-row']/div[@class='subject-cower col-sm-5']");

            var images = imagesNode?.SelectNodes(".//img");

            Cover[] Images;

            if (images?.Count > 0)
            {
                Images = new Cover[images.Count];

                for (int i = 0; i < images.Count; i++)
                {
                    Images[i] = new Cover(images[i].Attributes["data-full"]?.Value, images[i].Attributes["src"]?.Value, images[i].Attributes["data-thumb"]?.Value);
                }
            }
            else
            {
                Images = Array.Empty<Cover>();
            }

            return Images;
        }

        private IDataBase<string> GetDescription(HtmlNode mainNode, Uri uri = default)
        {
            var descNode = mainNode?.SelectSingleNode("./meta[@itemprop='description']");

            if (descNode != null)
            {
                return new DataBase<string>(Decode(descNode.Attributes["content"]?.Value), uri);
            }
            else
            {
                return null;
            }
        }

        private IDataBase<T>[] GetInfoData<T>(HtmlNode infoNode, string elemName)
        {
            var data = infoNode?.SelectNodes($".//span[contains(@class, '{elemName}')]");

            IDataBase<T>[] Data;

            if (data?.Count > 0)
            {
                Data = new DataBase<T>[data.Count];

                for (int i = 0; i < data.Count; i++)
                {
                    if (elemName == "elem_publisher ")
                    {
                        string name = Decode(data[i].InnerText);
                        string url = BaseUrl + $"/list/publisher/{base.Decode(data[i].InnerText)}";

                        IName nameData = new Name(new DataBase<string>(name, url));

                        Data[i] = new DataBase<T>((T)nameData, url);
                    }
                    else
                    {
                        string name = Decode(data[i].SelectSingleNode("./a")?.InnerText);
                        string url = BaseUrl + data[i].SelectSingleNode("./a")?.Attributes["href"]?.Value;

                        if (typeof(T) == typeof(IName))
                        {
                            IName nameData = new Name(new DataBase<string>(name, url));

                            Data[i] = new DataBase<T>((T)nameData, url);
                        }
                        else if (typeof(T) == typeof(DateTime))
                        {
                            DateTime.TryParseExact(name, "YYYY", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var result);

                            Data[i] = new DataBase<T>((T)(object)result, url);
                        }
                        else
                        {
                            Data[i] = new DataBase<T>((T)(object)name, url);
                        }
                    }
                }
            }
            else
            {
                Data = Array.Empty<DataBase<T>>();
            }

            return Data;
        }

        private IMangaObject GetMangaData(HtmlNode mainNode, Uri uri)
        {
            var infoNode = mainNode?.SelectSingleNode(".//div[@class='flex-row']/div[@class='subject-meta col-sm-7']");

            IName name = GetName(mainNode);

            IDataBase<string> description = GetDescription(mainNode);
            IDataBase<int> volumes = GetVolumes(infoNode);
            ICover[] covers = GetCovers(mainNode);
            IDataBase<IName>[] genres = GetInfoData<IName>(infoNode, "elem_genre ");
            IDataBase<IName>[] authors = GetInfoData<IName>(infoNode, "elem_author ");
            IDataBase<IName>[] writers = GetInfoData<IName>(infoNode, "elem_screenwriter ");
            IDataBase<IName>[] illustrators = GetInfoData<IName>(infoNode, "elem_illustrator ");
            IDataBase<IName>[] publishers = GetInfoData<IName>(infoNode, "elem_publisher ");
            IDataBase<IName>[] magazines = GetInfoData<IName>(infoNode, "elem_magazine ");
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

        private IEnumerable<IDataBase> GetMangaPages(string scriptText)
        {
            var index = scriptText.IndexOf(jsArrayVar);

            if (index != -1)
            {
                string array = scriptText.Substring(index + jsArrayVar.Length);

                if (array.Length > 0)
                {
                    Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);

                    MatchCollection matchs = regex.Matches(array);

                    if (matchs.Count > 0)
                    {
                        for (int i = 0; i < matchs.Count; i++)
                        {
                            yield return new DataBase(default, matchs[i].Groups[1].Value + matchs[i].Groups[3].Value);
                        }
                    }
                }
            }
        }

        private IName GetName(HtmlNode mainNode, Uri uri = default)
        {
            var namesNodes = mainNode?.SelectNodes("./h1[@class='names']/span");

            DataBase<string> local = default, eng = default, orig = default;

            Name name;

            if (namesNodes?.Count > 0)
            {
                for (int i = 0; i < namesNodes.Count; i++)
                {
                    switch (namesNodes[i].Attributes["class"]?.Value)
                    {
                        case "name":
                            local = new DataBase<string>(Decode(namesNodes[i].InnerText), uri);
                            break;

                        case "eng-name":
                            eng = new DataBase<string>(Decode(namesNodes[i].InnerText), uri);
                            break;

                        case "original-name":
                            orig = new DataBase<string>(Decode(namesNodes[i].InnerText), uri);
                            break;
                    }
                }
            }

            name = new Name(local, eng, orig);

            return name;
        }

        private IDataBase<int> GetVolumes(HtmlNode infoNode, Uri uri = default)
        {
            var volumesNode = infoNode?.SelectSingleNode("./p/text()[2]");

            if (volumesNode != null)
            {
                IDataBase<int> data;
                var volumes = Decode(volumesNode.InnerText);
                int.TryParse(volumes, out var result);
                data = new DataBase<int>(result, uri);
                return data;
            }
            else
            {
                return null;
            }
        }

        #endregion Data getting methods

        #endregion Private Methods

        #endregion Methods
    }
}