using HtmlAgilityPack;
using MangaParser.Models;
using MangaParser.Models.MangaData;
using MangaParser.Utilities.Helpers.Enums;
using System;
using System.Collections.Generic;
using System.Net;

namespace MangaParser.Parsers
{
    public class ReadMangaParser : Parser
    {
        protected override HtmlWeb Web { get; set; }
        public override string BaseUrl { get; protected set; }

        public ReadMangaParser(HtmlWeb web) : base(web)
        {
            BaseUrl = "readmanga.me";
        }

        public override MangaObject GetManga(string link)
        {
            if (!link.Contains(BaseUrl))
                return null;

            var htmlDoc = Web.Load(link);

            var mainNode = htmlDoc.DocumentNode.SelectSingleNode("//body/div[@class='pageBlock container']/div[@class='leftContent']");

            return GetMangaData(mainNode);
        }
        public override MangaObject GetManga(MangaTile manga)
        {
            if (!manga.MangaUrl.Contains(BaseUrl))
                return null;

            var htmlDoc = Web.Load(manga.MangaUrl);

            var mainNode = htmlDoc.DocumentNode.SelectSingleNode("//body/div[@class='pageBlock container']/div[@class='leftContent']");

            return GetMangaData(mainNode);
        }

        public override IEnumerable<MangaTile> SearchManga(string query)
        {
            query = query.Replace(' ', '+');

            var htmlDoc = Web.Load("http://" + BaseUrl + $"/search/advanced?q={query}", "POST");

            var mainNode = htmlDoc.DocumentNode.SelectSingleNode("//body/div[@class='pageBlock container']/div[@class='leftContent']/div[@id='mangaResults']");

            foreach (MangaTile item in GetSearchResult(mainNode))
            {
                yield return item;
            }
        }


        #region Search methods

        private IEnumerable<MangaTile> GetSearchResult(HtmlNode mainNode)
        {
            var tiles = mainNode.SelectNodes("./div[@class='tiles row']/div[@class='tile col-sm-6 ']");

            if (tiles != null)
            {
                for (int i = 0; i < tiles.Count; i++)
                {
                    var manga = new MangaTile(Decode(tiles[i].SelectSingleNode("./div[@class='desc']/h4")?.InnerText), Decode(tiles[i].SelectSingleNode("./div[@class='desc']/h3/a")?.InnerText), "http://" + BaseUrl + tiles[i].SelectSingleNode("./div[@class='desc']/h3/a")?.Attributes["href"]?.Value);

                    manga.Autors.AddRange(GetTileAutors(tiles[i]));
                    manga.Genres.AddRange(GetTileGenres(tiles[i]));
                    manga.Covers.AddRange(new Cover[] { new Cover(tiles[i].SelectSingleNode(".//img")?.Attributes["data-original"]?.Value) });

                    yield return manga;
                }
            }
        }

        private IEnumerable<DataBase> GetTileAutors(HtmlNode tileNode)
        {
            var autors = tileNode.SelectNodes(".//a[@class='person-link']");

            DataBase[] Autors;

            if (autors != null)
            {
                Autors = new DataBase[autors.Count];

                for (int i = 0; i < autors.Count; i++)
                {
                    Autors[i] = new DataBase
                    {
                        Value = Decode(autors[i].InnerText),
                        Link = autors[i].Attributes["href"]?.Value
                    };
                }
            }
            else
            {
                Autors = new DataBase[0];
            }

            return Autors;
        }

        private IEnumerable<DataBase> GetTileGenres(HtmlNode tileNode)
        {
            var genres = tileNode.SelectNodes(".//a[@class='element-link']");

            DataBase[] Genres;

            if (genres != null)
            {
                Genres = new DataBase[genres.Count];

                for (int i = 0; i < genres.Count; i++)
                {
                    Genres[i] = new DataBase
                    {
                        Value = Decode(genres[i].InnerText),
                        Link = genres[i].Attributes["href"]?.Value
                    };
                }
            }
            else
            {
                Genres = new DataBase[0];
            }

            return Genres;
        }

        #endregion

        #region Data getting methods

        private MangaObject GetMangaData(HtmlNode mainNode)
        {
            var infoNode = mainNode.SelectSingleNode(".//div[@class='flex-row']/div[@class='subject-meta col-sm-7']");

            MangaObject manga = new MangaObject(GetTitle(mainNode, MangaTitleType.Original), GetTitle(mainNode, MangaTitleType.English), GetTitle(mainNode, MangaTitleType.Localized), GetDescription(mainNode), GetVolumes(infoNode));

            manga.Covers.AddRange(GetCovers(mainNode));
            manga.Genres.AddRange(GetInfoData(infoNode, "elem_genre "));
            manga.Autors.AddRange(GetInfoData(infoNode, "elem_author "));
            manga.Writers.AddRange(GetInfoData(infoNode, "elem_screenwriter "));
            manga.Illustrators.AddRange(GetInfoData(infoNode, "elem_illustrator "));
            manga.Publishers.AddRange(GetInfoData(infoNode, "elem_publisher ", true));
            manga.Magazines.AddRange(GetInfoData(infoNode, "elem_magazine "));
            manga.ReleaseDate.AddRange(GetInfoData(infoNode, "elem_year "));
            manga.Chapters.AddRange(GetChapters(mainNode));

            return manga;
        }

        private string GetTitle(HtmlNode mainNode, MangaTitleType titleType)
        {
            var namesNodes = mainNode.SelectNodes("./h1[@class='names']/span");

            for (int i = 0; i < namesNodes.Count; i++)
            {
                switch (namesNodes[i].Attributes["class"]?.Value)
                {
                    case "name":
                        if (titleType == MangaTitleType.Localized)
                            return Decode(namesNodes[i].InnerText);
                        break;
                    case "eng-name":
                        if (titleType == MangaTitleType.English)
                            return Decode(namesNodes[i].InnerText);
                        break;
                    case "original-name":
                        if (titleType == MangaTitleType.Original)
                            return Decode(namesNodes[i].InnerText);
                        break;
                }
            }

            return null;
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
                return Decode(volumesNode.InnerText).Replace("\n", String.Empty).Replace("  ", String.Empty);
            else
                return null;
        }

        private IEnumerable<Cover> GetCovers(HtmlNode mainNode)
        {
            var imagesNode = mainNode.SelectSingleNode(".//div[@class='flex-row']/div[@class='subject-cower col-sm-5']");

            var images = imagesNode.SelectNodes(".//img");

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

        private IEnumerable<DataBase> GetInfoData(HtmlNode infoNode, string elemName, bool isPublisher = false)
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
                        Data[i] = new DataBase
                        {
                            Value = Decode(data[i].InnerText)
                        };

                        Data[i].Link = $"/list/publisher/{Data[i].Value}";
                    }
                    else
                    {
                        Data[i] = new DataBase
                        {
                            Value = Decode(data[i].SelectSingleNode("./a")?.InnerText),
                            Link = data[i].SelectSingleNode("./a")?.Attributes["href"]?.Value
                        };
                    }
                }
            }
            else
            {
                Data = new DataBase[0];
            }

            return Data;
        }

        private IEnumerable<ChapterData> GetChapters(HtmlNode mainNode)
        {
            var chapters = mainNode.SelectNodes("./div[contains(@class, 'chapters-link')]/table/tr");

            ChapterData[] Chapters;

            if (chapters != null)
            {
                Chapters = new ChapterData[chapters.Count];

                for (int i = chapters.Count - 1; i >= 0; i--)
                {
                    DateTime.TryParseExact(Decode(chapters[i].SelectSingleNode("./td[2]")?.InnerText), "dd.mm.yy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime date);

                    Chapters[chapters.Count - 1 - i] = new ChapterData
                    {
                        Value = Decode(chapters[i].SelectSingleNode("./td/a")?.InnerText).Replace("\n", String.Empty).Replace("  ", String.Empty),
                        Link = "http://" + BaseUrl + chapters[i].SelectSingleNode("./td/a")?.Attributes["href"]?.Value + "?mtr=1",
                        AddedDate = date,
                    };
                }
            }
            else
            {
                Chapters = new ChapterData[0];
            }

            return Chapters;
        }

        #endregion

        private string Decode(string htmlText) => htmlText != null ? WebUtility.HtmlDecode(htmlText).Trim() : null;
    }
}