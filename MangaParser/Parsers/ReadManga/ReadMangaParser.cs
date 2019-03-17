using HtmlAgilityPack;
using MangaParser.Core.Interfaces;
using MangaParser.Core.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System;

namespace MangaParser.Parsers.ReadManga
{
    public class ReadMangaParser : Parser, IParser
    {
        /// <summary>
        /// Pattern for js array scraping.
        /// </summary>
        private const string pattern = "\\[.*?'(http.*?)'\\,\\\"(.*?)\\\"";

        /// <summary>
        /// Js array name.
        /// </summary>
        private const string jsArrayVar = "rm_h.init";

        public ReadMangaParser(string baseUri = "http://readmanga.me") : base(baseUri)
        {
        }

        #region Public Methods

        public virtual IEnumerable<IMangaThumb> SearchManga(string query)
        {
            query = query.Replace(' ', '+');

            var htmlDoc = Web.Load(BaseUri + $"/search/advanced?q={query}", "POST");

            var mainNode = htmlDoc.DocumentNode.SelectSingleNode("//body/div[@class='pageBlock container']/div[@class='leftContent']/div[@id='mangaResults']");

            foreach (var item in GetSearchResult(mainNode))
            {
                yield return item;
            }
        }

        public virtual IManga GetManga(string mangaUri)
        {
            if (!mangaUri.Contains(BaseUri.Host))
                return null;

            var htmlDoc = Web.Load(mangaUri);

            var mainNode = htmlDoc.DocumentNode.SelectSingleNode("//body/div[@class='pageBlock container']/div[@class='leftContent']");

            var manga = GetMangaData(mainNode);

            manga.MangaUri = new Uri(mangaUri);

            return manga;
        }

        public virtual IManga GetManga(IMangaThumb mangaThumb)
        {
            if (!mangaThumb.MangaUri.OriginalString.Contains(BaseUri.Host))
                return null;

            var htmlDoc = Web.Load(mangaThumb.MangaUri);

            var mainNode = htmlDoc.DocumentNode.SelectSingleNode("//body/div[@class='pageBlock container']/div[@class='leftContent']");

            var manga = GetMangaData(mainNode);

            manga.MangaUri = mangaThumb.MangaUri;

            return manga;
        }

        public virtual IManga GetManga(Uri mangaUri)
        {
            if (!mangaUri.OriginalString.Contains(BaseUri.Host))
                return null;

            var htmlDoc = Web.Load(mangaUri);

            var mainNode = htmlDoc.DocumentNode.SelectSingleNode("//body/div[@class='pageBlock container']/div[@class='leftContent']");

            var manga = GetMangaData(mainNode);

            manga.MangaUri = mangaUri;

            return manga;
        }

        public virtual IEnumerable<IChapter> GetChapters(string mangaUri)
        {
            if (!mangaUri.Contains(BaseUri.Host))
                return null;

            var htmlDoc = Web.Load(mangaUri);

            var mainNode = htmlDoc.DocumentNode.SelectSingleNode("//body/div[@class='pageBlock container']/div[@class='leftContent']");

            return GetChapters(mainNode);
        }

        public virtual IEnumerable<IChapter> GetChapters(IMangaThumb manga)
        {
            if (!manga.MangaUri.OriginalString.Contains(BaseUri.Host))
                return null;

            var htmlDoc = Web.Load(manga.MangaUri);

            var mainNode = htmlDoc.DocumentNode.SelectSingleNode("//body/div[@class='pageBlock container']/div[@class='leftContent']");

            return GetChapters(mainNode);
        }

        public virtual IEnumerable<IChapter> GetChapters(Uri mangaUri)
        {
            if (!mangaUri.OriginalString.Contains(BaseUri.Host))
                return null;

            var htmlDoc = Web.Load(mangaUri);

            var mainNode = htmlDoc.DocumentNode.SelectSingleNode("//body/div[@class='pageBlock container']/div[@class='leftContent']");

            return GetChapters(mainNode);
        }

        public virtual IEnumerable<IPage> GetPages(string chapterUri)
        {
            if (!chapterUri.Contains(BaseUri.Host))
                yield return null;

            var htmlDoc = Web.Load(chapterUri);

            var scriptText = htmlDoc.DocumentNode.Descendants()?.Where(n => n.Name == "script" && n.InnerText.Contains(jsArrayVar)).First().InnerText;

            foreach (var item in GetMangaPages(scriptText))
            {
                yield return item;
            }
        }

        public virtual IEnumerable<IPage> GetPages(IChapter chapter)
        {
            if (!chapter.ChapterUri.OriginalString.Contains(BaseUri.Host))
                yield return null;

            var htmlDoc = Web.Load(chapter.ChapterUri);

            var scriptText = htmlDoc.DocumentNode.Descendants()?.Where(n => n.Name == "script" && n.InnerText.Contains(jsArrayVar)).First().InnerText;

            foreach (var item in GetMangaPages(scriptText))
            {
                yield return item;
            }
        }

        public virtual IEnumerable<IPage> GetPages(Uri chapterUri)
        {
            if (!chapterUri.OriginalString.Contains(BaseUri.Host))
                yield return null;

            var htmlDoc = Web.Load(chapterUri);

            var scriptText = htmlDoc.DocumentNode.Descendants()?.Where(n => n.Name == "script" && n.InnerText.Contains(jsArrayVar)).First().InnerText;

            foreach (var item in GetMangaPages(scriptText))
            {
                yield return item;
            }
        }

        #endregion Public Methods

        #region Search methods

        private IEnumerable<ReadMangaObject> GetSearchResult(HtmlNode mainNode)
        {
            var thumbs = mainNode.SelectNodes("./div[@class='tiles row']/div[@class='tile col-sm-6 ']");

            if (thumbs != null)
            {
                for (int i = 0; i < thumbs.Count; i++)
                {
                    var manga = new ReadMangaObject
                    {
                        Name = new Name(null, Decode(thumbs[i].SelectSingleNode("./div[@class='desc']/h4")?.InnerText), Decode(thumbs[i].SelectSingleNode("./div[@class='desc']/h3/a")?.InnerText)),
                        MangaUri = new Uri(BaseUri, thumbs[i].SelectSingleNode("./div[@class='desc']/h3/a")?.Attributes["href"]?.Value),
                        Autors = GetThumbAutors(thumbs[i]),
                        Genres = GetThumbGenres(thumbs[i]),
                        Covers = new Cover[] { new Cover(thumbs[i].SelectSingleNode(".//img")?.Attributes["data-original"]?.Value) }
                    };

                    yield return manga;
                }
            }
        }

        private DataBase[] GetThumbAutors(HtmlNode tileNode)
        {
            var autors = tileNode.SelectNodes(".//a[@class='person-link']");

            DataBase[] Autors;

            if (autors != null)
            {
                Autors = new DataBase[autors.Count];

                for (int i = 0; i < autors.Count; i++)
                {
                    Autors[i] = new DataBase(Decode(autors[i].InnerText), BaseUri + autors[i].Attributes["href"]?.Value);
                }
            }
            else
            {
                Autors = new DataBase[0];
            }

            return Autors;
        }

        private DataBase[] GetThumbGenres(HtmlNode tileNode)
        {
            var genres = tileNode.SelectNodes(".//a[@class='element-link']");

            DataBase[] Genres;

            if (genres != null)
            {
                Genres = new DataBase[genres.Count];

                for (int i = 0; i < genres.Count; i++)
                {
                    Genres[i] = new DataBase(Decode(genres[i].InnerText), BaseUri + genres[i].Attributes["href"]?.Value);
                }
            }
            else
            {
                Genres = new DataBase[0];
            }

            return Genres;
        }

        #endregion Search methods

        #region Data getting methods

        private ReadMangaObject GetMangaData(HtmlNode mainNode)
        {
            var infoNode = mainNode.SelectSingleNode(".//div[@class='flex-row']/div[@class='subject-meta col-sm-7']");

            var manga = new ReadMangaObject
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

        private IEnumerable<Page> GetMangaPages(string scriptText)
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
                            yield return new Page(matchs[i].Groups[1].Value + matchs[i].Groups[2].Value);
                        }
                    }
                }
            }
        }

        private Name GetName(HtmlNode mainNode)
        {
            var namesNodes = mainNode.SelectNodes("./h1[@class='names']/span");

            string local = default(string), eng = default(string), orig = default(string);

            Name name;

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

            name = new Name(orig, eng, local);

            return name;
        }

        private string GetDescription(HtmlNode mainNode)
        {
            var descNode = mainNode.SelectSingleNode("./meta[@itemprop='description']");

            if (descNode != null)
                return Decode(descNode.Attributes["content"]?.Value);
            else
                return null;
        }

        private string GetVolumes(HtmlNode infoNode)
        {
            var volumesNode = infoNode.SelectSingleNode("./p/text()[2]");

            if (volumesNode != null)
                return Decode(volumesNode.InnerText);
            else
                return null;
        }

        private Cover[] GetCovers(HtmlNode mainNode)
        {
            var imagesNode = mainNode.SelectSingleNode(".//div[@class='flex-row']/div[@class='subject-cower col-sm-5']");

            var images = imagesNode?.SelectNodes(".//img");

            Cover[] Images;

            if (images != null)
            {
                Images = new Cover[images.Count];

                for (int i = 0; i < images.Count; i++)
                {
                    Images[i] = new Cover(images[i].Attributes["data-thumb"]?.Value, images[i].Attributes["src"]?.Value, images[i].Attributes["data-full"]?.Value);
                }
            }
            else
            {
                Images = new Cover[0];
            }

            return Images;
        }

        private DataBase[] GetInfoData(HtmlNode infoNode, string elemName, bool isPublisher = false)
        {
            var data = infoNode.SelectNodes($".//span[contains(@class, '{elemName}')]");

            DataBase[] Data;

            if (data != null)
            {
                Data = new DataBase[data.Count];

                for (int i = 0; i < data.Count; i++)
                {
                    if (isPublisher)
                    {
                        Data[i] = new DataBase(Decode(data[i].InnerText), BaseUri + $"/list/publisher/{Decode(data[i].InnerText)}");
                    }
                    else
                    {
                        Data[i] = new DataBase(Decode(data[i].SelectSingleNode("./a")?.InnerText), BaseUri + data[i].SelectSingleNode("./a")?.Attributes["href"]?.Value);
                    }
                }
            }
            else
            {
                Data = new DataBase[0];
            }

            return Data;
        }

        private MangaChapter[] GetChapters(HtmlNode mainNode)
        {
            var chapters = mainNode.SelectNodes("./div[contains(@class, 'chapters-link')]/table/tr");

            MangaChapter[] Chapters;

            if (chapters != null)
            {
                Chapters = new MangaChapter[chapters.Count];

                for (int i = chapters.Count - 1; i >= 0; i--)
                {
                    DateTime.TryParseExact(Decode(chapters[i].SelectSingleNode("./td[2]")?.InnerText), "dd.mm.yy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime date);

                    Chapters[chapters.Count - 1 - i] = new MangaChapter(Decode(chapters[i].SelectSingleNode("./td/a")?.InnerText), BaseUri + chapters[i].SelectSingleNode("./td/a")?.Attributes["href"]?.Value + "?mtr=1", date);
                }
            }
            else
            {
                Chapters = new MangaChapter[0];
            }

            return Chapters;
        }

        #endregion Data getting methods
    }
}