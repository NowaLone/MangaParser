using HtmlAgilityPack;
using MangaParser.Core.Interfaces;
using MangaParser.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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

        public ReadMangaParser(string baseUrl = "https://readmanga.me") : base(baseUrl)
        {
        }

        #endregion Constructors

        #region Methods

        #region Search

        protected override IEnumerable<IMangaThumb> SearchMangaCore(HtmlDocument htmlDoc)
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

        public override IManga GetManga(Uri url)
        {
            if (url is null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            var htmlDoc = Web.Load(url);

            var manga = GetMangaCore(htmlDoc);

            (manga as MangaObject).MangaUri = url;

            return manga;
        }

        public override async Task<IManga> GetMangaAsync(Uri url)
        {
            if (url is null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            var htmlDoc = await Web.LoadFromWebAsync(url.OriginalString).ConfigureAwait(false);

            var manga = GetMangaCore(htmlDoc);

            (manga as MangaObject).MangaUri = url;

            return manga;
        }

        protected override IManga GetMangaCore(HtmlDocument htmlDoc)
        {
            if (htmlDoc is null)
            {
                throw new ArgumentNullException(nameof(htmlDoc));
            }

            var mainNode = htmlDoc.DocumentNode.SelectSingleNode("//body/div[@class='pageBlock container']/div[@class='leftContent']");

            return GetMangaData(mainNode);
        }

        #endregion GetManga

        #region GetChapters

        protected override IEnumerable<IChapter> GetChaptersCore(HtmlDocument htmlDoc)
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

        protected override IEnumerable<IPage> GetPagesCore(HtmlDocument htmlDoc)
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

        private IEnumerable<MangaObject> GetSearchResult(HtmlNode mainNode)
        {
            var thumbs = mainNode?.SelectNodes("./div[@class='tiles row']/div[@class='tile col-sm-6 ']");

            if (thumbs != null)
            {
                for (int i = 0; i < thumbs.Count; i++)
                {
                    var manga = new MangaObject
                    {
                        Name = new MangaName(Decode(thumbs[i].SelectSingleNode("./div[@class='desc']/h4")?.InnerText), Decode(thumbs[i].SelectSingleNode("./div[@class='desc']/h3/a")?.InnerText), null),
                        MangaUri = new Uri(BaseUrl, thumbs[i].SelectSingleNode("./div[@class='desc']/h3/a")?.Attributes["href"]?.Value),
                        Autors = GetThumbAutors(thumbs[i]),
                        Genres = GetThumbGenres(thumbs[i]),
                        Covers = new MangaCover[] { new MangaCover(thumbs[i].SelectSingleNode(".//img")?.Attributes["data-original"]?.Value) }
                    };

                    yield return manga;
                }
            }
        }

        private MangaDataBase[] GetThumbAutors(HtmlNode tileNode)
        {
            var autors = tileNode?.SelectNodes(".//a[@class='person-link']");

            MangaDataBase[] Autors;

            if (autors != null)
            {
                Autors = new MangaDataBase[autors.Count];

                for (int i = 0; i < autors.Count; i++)
                {
                    Autors[i] = new MangaDataBase(Decode(autors[i].InnerText), BaseUrl + autors[i].Attributes["href"]?.Value);
                }
            }
            else
            {
                Autors = new MangaDataBase[0];
            }

            return Autors;
        }

        private MangaDataBase[] GetThumbGenres(HtmlNode tileNode)
        {
            var genres = tileNode?.SelectNodes(".//a[@class='element-link']");

            MangaDataBase[] Genres;

            if (genres != null)
            {
                Genres = new MangaDataBase[genres.Count];

                for (int i = 0; i < genres.Count; i++)
                {
                    Genres[i] = new MangaDataBase(Decode(genres[i].InnerText), BaseUrl + genres[i].Attributes["href"]?.Value);
                }
            }
            else
            {
                Genres = new MangaDataBase[0];
            }

            return Genres;
        }

        #endregion Search methods

        #region Data getting methods

        private IEnumerable<IChapter> GetChapters(HtmlNode mainNode)
        {
            var chapters = mainNode?.SelectNodes("./div[contains(@class, 'chapters-link')]/table/tr");

            MangaChapter[] Chapters;

            if (chapters != null)
            {
                Chapters = new MangaChapter[chapters.Count];

                for (int i = chapters.Count - 1; i >= 0; i--)
                {
                    DateTime.TryParseExact(Decode(chapters[i].SelectSingleNode("./td[2]")?.InnerText), "dd.MM.yy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime date);

                    Chapters[chapters.Count - 1 - i] = new MangaChapter(Decode(chapters[i].SelectSingleNode("./td/a")?.InnerText), BaseUrl + chapters[i].SelectSingleNode("./td/a")?.Attributes["href"]?.Value + "?mtr=1", date);
                }
            }
            else
            {
                Chapters = new MangaChapter[0];
            }

            return Chapters;
        }

        private MangaCover[] GetCovers(HtmlNode mainNode)
        {
            var imagesNode = mainNode?.SelectSingleNode(".//div[@class='flex-row']/div[@class='subject-cower col-sm-5']");

            var images = imagesNode?.SelectNodes(".//img");

            MangaCover[] Images;

            if (images != null)
            {
                Images = new MangaCover[images.Count];

                for (int i = 0; i < images.Count; i++)
                {
                    Images[i] = new MangaCover(images[i].Attributes["data-thumb"]?.Value, images[i].Attributes["src"]?.Value, images[i].Attributes["data-full"]?.Value);
                }
            }
            else
            {
                Images = new MangaCover[0];
            }

            return Images;
        }

        private string GetDescription(HtmlNode mainNode)
        {
            var descNode = mainNode?.SelectSingleNode("./meta[@itemprop='description']");

            if (descNode != null)
                return Decode(descNode.Attributes["content"]?.Value);
            else
                return null;
        }

        private MangaDataBase[] GetInfoData(HtmlNode infoNode, string elemName, bool isPublisher = false)
        {
            var data = infoNode?.SelectNodes($".//span[contains(@class, '{elemName}')]");

            MangaDataBase[] Data;

            if (data != null)
            {
                Data = new MangaDataBase[data.Count];

                for (int i = 0; i < data.Count; i++)
                {
                    if (isPublisher)
                    {
                        Data[i] = new MangaDataBase(Decode(data[i].InnerText), BaseUrl + $"/list/publisher/{Decode(data[i].InnerText)}");
                    }
                    else
                    {
                        Data[i] = new MangaDataBase(Decode(data[i].SelectSingleNode("./a")?.InnerText), BaseUrl + data[i].SelectSingleNode("./a")?.Attributes["href"]?.Value);
                    }
                }
            }
            else
            {
                Data = new MangaDataBase[0];
            }

            return Data;
        }

        private MangaObject GetMangaData(HtmlNode mainNode)
        {
            var infoNode = mainNode?.SelectSingleNode(".//div[@class='flex-row']/div[@class='subject-meta col-sm-7']");

            var manga = new MangaObject
            {
                Name = GetName(mainNode),
                Description = GetDescription(mainNode),
                Volumes = GetVolumes(infoNode),
                Covers = GetCovers(mainNode),
                Genres = GetInfoData(infoNode, "elem_genre "),
                Autors = GetInfoData(infoNode, "elem_author "),
                Writers = GetInfoData(infoNode, "elem_screenwriter "),
                Illustrators = GetInfoData(infoNode, "elem_illustrator "),
                Publishers = GetInfoData(infoNode, "elem_publisher ", true),
                Magazines = GetInfoData(infoNode, "elem_magazine "),
                ReleaseDate = GetInfoData(infoNode, "elem_year "),
            };

            return manga;
        }

        private IEnumerable<MangaPage> GetMangaPages(string scriptText)
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
                            yield return new MangaPage(matchs[i].Groups[1].Value + matchs[i].Groups[3].Value);
                        }
                    }
                }
            }
        }

        private MangaName GetName(HtmlNode mainNode)
        {
            var namesNodes = mainNode?.SelectNodes("./h1[@class='names']/span");

            string local = default(string), eng = default(string), orig = default(string);

            MangaName name;

            if (namesNodes != null)
            {
                for (int i = 0; i < namesNodes.Count; i++)
                {
                    switch (namesNodes[i].Attributes["class"]?.Value)
                    {
                        case "name":
                            local = Decode(namesNodes[i].InnerText);
                            break;

                        case "eng-name":
                            eng = Decode(namesNodes[i].InnerText);
                            break;

                        case "original-name":
                            orig = Decode(namesNodes[i].InnerText);
                            break;
                    }
                }
            }

            name = new MangaName(eng, local, orig);

            return name;
        }

        private string GetVolumes(HtmlNode infoNode)
        {
            var volumesNode = infoNode?.SelectSingleNode("./p/text()[2]");

            if (volumesNode != null)
                return Decode(volumesNode.InnerText);
            else
                return null;
        }

        #endregion Data getting methods

        #endregion Private Methods

        #endregion Methods
    }
}