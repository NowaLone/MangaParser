using MangaParser.Core.Interfaces;
using MangaParser.Core.Models;
using MangaParser.Parsers.HtmlWebParsers.ReadManga;
using MangaParser.Parsers.Tests.Attributes;
using MangaParser.Parsers.Tests.Converters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MangaParser.Parsers.Tests.HtmlWebParsers.ReadManga
{
    [TestClass]
    [TestClassCategory("Parser", "ReadManga")]
    public class ReadMangaParserTests : CoreParserTests
    {
        #region Properties

        protected override string ParserPath => "ReadManga";
        protected override IParser GetNewParser => new ReadMangaParser();

        #endregion Properties

        #region Search

        #region Sync

        [DataTestMethod]
        [TestMethodCategory("Search")]
        [TestMethodDelay()]
        [DataRow("sakamoto", DisplayName = "'sakamoto' query")]
        [DataRow("akame", DisplayName = "'akame' query")]
        [DataRow("Castle Town Dandelion", DisplayName = "Multiple word query")]
        [DataRow("asdfasedfqewqfeqwfq", DisplayName = "Rubbish query")]
        [DataRow("", DisplayName = "Empty query")]
        [DataRow(null, DisplayName = "Null query")]
        public virtual void SearchManga_WithQuery_ShouldReturn_CorrectData(string query)
        {
            // arrange
            IParser parser = GetNewParser;
            List<IMangaObject> data = new List<IMangaObject>();

            string dataPath = GetDataPath(ParserPath, "Search", query);

            if (File.Exists(dataPath))
            {
                using (var reader = new StreamReader(dataPath))
                {
                    string json = reader.ReadToEnd();
                    data = JsonConvert.DeserializeObject<List<IMangaObject>>(json, new FromJsonToDataConverter());
                }
            }

            // act
            var result = parser.SearchManga(query).ToList();

            // assert
            CollectionAssert.AreEquivalent(data, result);

            foreach (IMangaObject resultObj in result)
            {
                IMangaObject dataObj = data.First(o => o.Url.Equals(resultObj.Url));

                CollectionAssert.AreEquivalent(dataObj.Covers.ToList(), resultObj.Covers.ToList());
                CollectionAssert.AreEquivalent(dataObj.Authors.ToList(), resultObj.Authors.ToList());

                // Looks like the readmanga search returns 3 random genres
                // TODO: need to think about how to make it testable
                //CollectionAssert.AreEquivalent(dataObj.Genres.ToList(), resultObj.Genres.ToList());

                Assert.AreEqual(dataObj.Description, resultObj.Description);
                CollectionAssert.AreEquivalent(dataObj.Illustrators.ToList(), resultObj.Illustrators.ToList());
                CollectionAssert.AreEquivalent(dataObj.Writers.ToList(), resultObj.Writers.ToList());
                CollectionAssert.AreEquivalent(dataObj.Magazines.ToList(), resultObj.Magazines.ToList());
                CollectionAssert.AreEquivalent(dataObj.Publishers.ToList(), resultObj.Publishers.ToList());
                Assert.AreEqual(dataObj.ReleaseDate, resultObj.ReleaseDate);
                Assert.AreEqual(dataObj.Volumes, resultObj.Volumes);
            }
        }

        #endregion Sync

        #region Async

        [DataTestMethod]
        [TestMethodCategory("Search", "Async")]
        [TestMethodDelay()]
        [DataRow("sakamoto", DisplayName = "'sakamoto' query")]
        [DataRow("akame", DisplayName = "'akame' query")]
        [DataRow("Castle Town Dandelion", DisplayName = "Multiple word query")]
        [DataRow("asdfasedfqewqfeqwfq", DisplayName = "Rubbish query")]
        [DataRow("", DisplayName = "Empty query")]
        [DataRow(null, DisplayName = "Null query")]
        public virtual async Task SearchMangaAsync_WithQuery_ShouldReturn_CorrectData(string query)
        {
            // arrange
            IParser parser = GetNewParser;
            List<IMangaObject> data = new List<IMangaObject>();

            string dataPath = GetDataPath(ParserPath, "Search", query);

            if (File.Exists(dataPath))
            {
                using (var reader = new StreamReader(dataPath))
                {
                    string json = reader.ReadToEnd();
                    data = JsonConvert.DeserializeObject<List<IMangaObject>>(json, new FromJsonToDataConverter());
                }
            }

            // act
            var result = (await parser.SearchMangaAsync(query)).ToList();

            // assert
            CollectionAssert.AreEquivalent(data, result);

            foreach (IMangaObject resultObj in result)
            {
                IMangaObject dataObj = data.First(o => o.Url.Equals(resultObj.Url));

                CollectionAssert.AreEquivalent(dataObj.Covers.ToList(), resultObj.Covers.ToList());
                CollectionAssert.AreEquivalent(dataObj.Authors.ToList(), resultObj.Authors.ToList());

                // Looks like the readmanga search returns 3 random genres
                // TODO: need to think about how to make it testable
                //CollectionAssert.AreEquivalent(dataObj.Genres.ToList(), resultObj.Genres.ToList());

                Assert.AreEqual(dataObj.Description, resultObj.Description);
                CollectionAssert.AreEquivalent(dataObj.Illustrators.ToList(), resultObj.Illustrators.ToList());
                CollectionAssert.AreEquivalent(dataObj.Writers.ToList(), resultObj.Writers.ToList());
                CollectionAssert.AreEquivalent(dataObj.Magazines.ToList(), resultObj.Magazines.ToList());
                CollectionAssert.AreEquivalent(dataObj.Publishers.ToList(), resultObj.Publishers.ToList());
                Assert.AreEqual(dataObj.ReleaseDate, resultObj.ReleaseDate);
                Assert.AreEqual(dataObj.Volumes, resultObj.Volumes);
            }
        }

        #endregion Async

        #endregion Search

        #region GetManga

        #region Sync

        [DataTestMethod]
        [TestMethodCategory("GetManga")]
        [TestMethodDelay()]
        [DataRow("https://readmanga.live/haven_t_you_heard__i_m_sakamoto", DisplayName = "'Haven't You Heard? I'm Sakamoto' url")]
        [DataRow("https://readmanga.live/legenda_o_serebrianom_klyke_akame", DisplayName = "'Legend of Silver Fang Akame' url")]
        [DataRow("https://readmanga.live/castle_town_dandelion", DisplayName = "'Castle Town Dandelion' url")]
        [DataRow("https://readmanga.live/one_punch_man__A1bc88e", DisplayName = "'One Punch-Man' url")]
        public override void GetManga_ByUrlString_ShouldReturn_CorrectData(string url)
        {
            // arrange
            IParser parser = GetNewParser;
            IMangaObject data = null;

            string dataPath = GetDataPath(ParserPath, "GetManga", Path.GetFileName(url));

            if (File.Exists(dataPath))
            {
                using (var reader = new StreamReader(dataPath))
                {
                    string json = reader.ReadToEnd();
                    data = JsonConvert.DeserializeObject<IMangaObject>(json, new FromJsonToDataConverter());
                }
            }

            // act
            var result = parser.GetManga(url);

            // assert
            Assert.AreEqual(data, result);
            CollectionAssert.AreEquivalent(data.Covers.ToList(), result.Covers.ToList());
            CollectionAssert.AreEquivalent(data.Authors.ToList(), result.Authors.ToList());
            CollectionAssert.AreEquivalent(data.Genres.ToList(), result.Genres.ToList());
            Assert.AreEqual(data.Description, result.Description);
            CollectionAssert.AreEquivalent(data.Illustrators.ToList(), result.Illustrators.ToList());
            CollectionAssert.AreEquivalent(data.Writers.ToList(), result.Writers.ToList());
            CollectionAssert.AreEquivalent(data.Magazines.ToList(), result.Magazines.ToList());
            CollectionAssert.AreEquivalent(data.Publishers.ToList(), result.Publishers.ToList());
            Assert.AreEqual(data.ReleaseDate, result.ReleaseDate);
            Assert.AreEqual(data.Volumes, result.Volumes);
        }

        [DataTestMethod]
        [TestMethodCategory("GetManga")]
        [TestMethodDelay()]
        [DataRow("https://readmanga.live/haven_t_you_heard__i_m_sakamoto", DisplayName = "'Haven't You Heard? I'm Sakamoto' url")]
        [DataRow("https://readmanga.live/legenda_o_serebrianom_klyke_akame", DisplayName = "'Legend of Silver Fang Akame' url")]
        [DataRow("https://readmanga.live/castle_town_dandelion", DisplayName = "'Castle Town Dandelion' url")]
        [DataRow("https://readmanga.live/one_punch_man__A1bc88e", DisplayName = "'One Punch-Man' url")]
        public override void GetManga_ByUri_ShouldReturn_CorrectData(string url)
        {
            // arrange
            Uri uri = new Uri(url);

            IParser parser = GetNewParser;
            IMangaObject data = null;

            string dataPath = GetDataPath(ParserPath, "GetManga", Path.GetFileName(url));

            if (File.Exists(dataPath))
            {
                using (var reader = new StreamReader(dataPath))
                {
                    string json = reader.ReadToEnd();
                    data = JsonConvert.DeserializeObject<IMangaObject>(json, new FromJsonToDataConverter());
                }
            }

            // act
            var result = parser.GetManga(uri);

            // assert
            Assert.AreEqual(data, result);
            CollectionAssert.AreEquivalent(data.Covers.ToList(), result.Covers.ToList());
            CollectionAssert.AreEquivalent(data.Authors.ToList(), result.Authors.ToList());
            CollectionAssert.AreEquivalent(data.Genres.ToList(), result.Genres.ToList());
            Assert.AreEqual(data.Description, result.Description);
            CollectionAssert.AreEquivalent(data.Illustrators.ToList(), result.Illustrators.ToList());
            CollectionAssert.AreEquivalent(data.Writers.ToList(), result.Writers.ToList());
            CollectionAssert.AreEquivalent(data.Magazines.ToList(), result.Magazines.ToList());
            CollectionAssert.AreEquivalent(data.Publishers.ToList(), result.Publishers.ToList());
            Assert.AreEqual(data.ReleaseDate, result.ReleaseDate);
            Assert.AreEqual(data.Volumes, result.Volumes);
        }

        [DataTestMethod]
        [TestMethodCategory("GetManga")]
        [TestMethodDelay()]
        [DataRow("https://readmanga.live/haven_t_you_heard__i_m_sakamoto", DisplayName = "'Haven't You Heard? I'm Sakamoto' url")]
        [DataRow("https://readmanga.live/legenda_o_serebrianom_klyke_akame", DisplayName = "'Legend of Silver Fang Akame' url")]
        [DataRow("https://readmanga.live/castle_town_dandelion", DisplayName = "'Castle Town Dandelion' url")]
        [DataRow("https://readmanga.live/one_punch_man__A1bc88e", DisplayName = "'One Punch-Man' url")]
        public override void GetManga_ByMangaObject_ShouldReturn_CorrectData(string url)
        {
            // arrange
            IMangaObject manga = new MangaObject(null, url);

            IParser parser = GetNewParser;
            IMangaObject data = null;

            string dataPath = GetDataPath(ParserPath, "GetManga", Path.GetFileName(url));

            if (File.Exists(dataPath))
            {
                using (var reader = new StreamReader(dataPath))
                {
                    string json = reader.ReadToEnd();
                    data = JsonConvert.DeserializeObject<IMangaObject>(json, new FromJsonToDataConverter());
                }
            }

            // act
            var result = parser.GetManga(manga);

            // assert
            Assert.AreEqual(data, result);
            CollectionAssert.AreEquivalent(data.Covers.ToList(), result.Covers.ToList());
            CollectionAssert.AreEquivalent(data.Authors.ToList(), result.Authors.ToList());
            CollectionAssert.AreEquivalent(data.Genres.ToList(), result.Genres.ToList());
            Assert.AreEqual(data.Description, result.Description);
            CollectionAssert.AreEquivalent(data.Illustrators.ToList(), result.Illustrators.ToList());
            CollectionAssert.AreEquivalent(data.Writers.ToList(), result.Writers.ToList());
            CollectionAssert.AreEquivalent(data.Magazines.ToList(), result.Magazines.ToList());
            CollectionAssert.AreEquivalent(data.Publishers.ToList(), result.Publishers.ToList());
            Assert.AreEqual(data.ReleaseDate, result.ReleaseDate);
            Assert.AreEqual(data.Volumes, result.Volumes);
        }

        #endregion Sync

        #region Async

        [DataTestMethod]
        [TestMethodCategory("GetManga", "Async")]
        [TestMethodDelay()]
        [DataRow("https://readmanga.live/haven_t_you_heard__i_m_sakamoto", DisplayName = "'Haven't You Heard? I'm Sakamoto' url")]
        [DataRow("https://readmanga.live/legenda_o_serebrianom_klyke_akame", DisplayName = "'Legend of Silver Fang Akame' url")]
        [DataRow("https://readmanga.live/castle_town_dandelion", DisplayName = "'Castle Town Dandelion' url")]
        [DataRow("https://readmanga.live/one_punch_man__A1bc88e", DisplayName = "'One Punch-Man' url")]
        public override async Task GetMangaAsync_ByUrlString_ShouldReturn_CorrectData(string url)
        {
            // arrange
            IParser parser = GetNewParser;
            IMangaObject data = null;

            string dataPath = GetDataPath(ParserPath, "GetManga", Path.GetFileName(url));

            if (File.Exists(dataPath))
            {
                using (var reader = new StreamReader(dataPath))
                {
                    string json = reader.ReadToEnd();
                    data = JsonConvert.DeserializeObject<IMangaObject>(json, new FromJsonToDataConverter());
                }
            }

            // act
            var result = await parser.GetMangaAsync(url);

            // assert
            Assert.AreEqual(data, result);
            CollectionAssert.AreEquivalent(data.Covers.ToList(), result.Covers.ToList());
            CollectionAssert.AreEquivalent(data.Authors.ToList(), result.Authors.ToList());
            CollectionAssert.AreEquivalent(data.Genres.ToList(), result.Genres.ToList());
            Assert.AreEqual(data.Description, result.Description);
            CollectionAssert.AreEquivalent(data.Illustrators.ToList(), result.Illustrators.ToList());
            CollectionAssert.AreEquivalent(data.Writers.ToList(), result.Writers.ToList());
            CollectionAssert.AreEquivalent(data.Magazines.ToList(), result.Magazines.ToList());
            CollectionAssert.AreEquivalent(data.Publishers.ToList(), result.Publishers.ToList());
            Assert.AreEqual(data.ReleaseDate, result.ReleaseDate);
            Assert.AreEqual(data.Volumes, result.Volumes);
        }

        [DataTestMethod]
        [TestMethodCategory("GetManga", "Async")]
        [TestMethodDelay()]
        [DataRow("https://readmanga.live/haven_t_you_heard__i_m_sakamoto", DisplayName = "'Haven't You Heard? I'm Sakamoto' url")]
        [DataRow("https://readmanga.live/legenda_o_serebrianom_klyke_akame", DisplayName = "'Legend of Silver Fang Akame' url")]
        [DataRow("https://readmanga.live/castle_town_dandelion", DisplayName = "'Castle Town Dandelion' url")]
        [DataRow("https://readmanga.live/one_punch_man__A1bc88e", DisplayName = "'One Punch-Man' url")]
        public override async Task GetMangaAsync_ByUri_ShouldReturn_CorrectData(string url)
        {
            // arrange
            Uri uri = new Uri(url);

            IParser parser = GetNewParser;
            IMangaObject data = null;

            string dataPath = GetDataPath(ParserPath, "GetManga", Path.GetFileName(url));

            if (File.Exists(dataPath))
            {
                using (var reader = new StreamReader(dataPath))
                {
                    string json = reader.ReadToEnd();
                    data = JsonConvert.DeserializeObject<IMangaObject>(json, new FromJsonToDataConverter());
                }
            }

            // act
            var result = await parser.GetMangaAsync(uri);

            // assert
            Assert.AreEqual(data, result);
            CollectionAssert.AreEquivalent(data.Covers.ToList(), result.Covers.ToList());
            CollectionAssert.AreEquivalent(data.Authors.ToList(), result.Authors.ToList());
            CollectionAssert.AreEquivalent(data.Genres.ToList(), result.Genres.ToList());
            Assert.AreEqual(data.Description, result.Description);
            CollectionAssert.AreEquivalent(data.Illustrators.ToList(), result.Illustrators.ToList());
            CollectionAssert.AreEquivalent(data.Writers.ToList(), result.Writers.ToList());
            CollectionAssert.AreEquivalent(data.Magazines.ToList(), result.Magazines.ToList());
            CollectionAssert.AreEquivalent(data.Publishers.ToList(), result.Publishers.ToList());
            Assert.AreEqual(data.ReleaseDate, result.ReleaseDate);
            Assert.AreEqual(data.Volumes, result.Volumes);
        }

        [DataTestMethod]
        [TestMethodCategory("GetManga", "Async")]
        [TestMethodDelay()]
        [DataRow("https://readmanga.live/haven_t_you_heard__i_m_sakamoto", DisplayName = "'Haven't You Heard? I'm Sakamoto' url")]
        [DataRow("https://readmanga.live/legenda_o_serebrianom_klyke_akame", DisplayName = "'Legend of Silver Fang Akame' url")]
        [DataRow("https://readmanga.live/castle_town_dandelion", DisplayName = "'Castle Town Dandelion' url")]
        [DataRow("https://readmanga.live/one_punch_man__A1bc88e", DisplayName = "'One Punch-Man' url")]
        public override async Task GetMangaAsync_ByMangaObject_ShouldReturn_CorrectData(string url)
        {
            // arrange
            IMangaObject manga = new MangaObject(null, url);

            IParser parser = GetNewParser;
            IMangaObject data = null;

            string dataPath = GetDataPath(ParserPath, "GetManga", Path.GetFileName(url));

            if (File.Exists(dataPath))
            {
                using (var reader = new StreamReader(dataPath))
                {
                    string json = reader.ReadToEnd();
                    data = JsonConvert.DeserializeObject<IMangaObject>(json, new FromJsonToDataConverter());
                }
            }

            // act
            var result = await parser.GetMangaAsync(manga);

            // assert
            Assert.AreEqual(data, result);
            CollectionAssert.AreEquivalent(data.Covers.ToList(), result.Covers.ToList());
            CollectionAssert.AreEquivalent(data.Authors.ToList(), result.Authors.ToList());
            CollectionAssert.AreEquivalent(data.Genres.ToList(), result.Genres.ToList());
            Assert.AreEqual(data.Description, result.Description);
            CollectionAssert.AreEquivalent(data.Illustrators.ToList(), result.Illustrators.ToList());
            CollectionAssert.AreEquivalent(data.Writers.ToList(), result.Writers.ToList());
            CollectionAssert.AreEquivalent(data.Magazines.ToList(), result.Magazines.ToList());
            CollectionAssert.AreEquivalent(data.Publishers.ToList(), result.Publishers.ToList());
            Assert.AreEqual(data.ReleaseDate, result.ReleaseDate);
            Assert.AreEqual(data.Volumes, result.Volumes);
        }

        #endregion Async

        #endregion GetManga

        #region GetChapters

        #region Sync

        [DataTestMethod]
        [TestMethodCategory("GetChapters")]
        [TestMethodDelay()]
        [DataRow("https://readmanga.live/haven_t_you_heard__i_m_sakamoto", DisplayName = "'Haven't You Heard? I'm Sakamoto' chapters")]
        [DataRow("https://readmanga.live/legenda_o_serebrianom_klyke_akame", DisplayName = "'Legend of Silver Fang Akame' chapters")]
        [DataRow("https://readmanga.live/castle_town_dandelion", DisplayName = "'Castle Town Dandelion' chapters")]
        public override void GetChapters_ByUrlString_ShouldReturn_CorrectData(string url)
        {
            // arrange
            IParser parser = GetNewParser;
            List<IChapter> data = new List<IChapter>();

            string dataPath = GetDataPath(ParserPath, "GetChapters", Path.GetFileName(url));

            if (File.Exists(dataPath))
            {
                using (var reader = new StreamReader(dataPath))
                {
                    string json = reader.ReadToEnd();
                    data = JsonConvert.DeserializeObject<List<IChapter>>(json, new FromJsonToDataConverter());
                }
            }

            // act
            var result = parser.GetChapters(url).ToList();

            // assert
            CollectionAssert.AreEquivalent(data, result);

            foreach (IChapter resultObj in result)
            {
                IChapter dataObj = data.First(o => o.Url.Equals(resultObj.Url));

                Assert.AreEqual(dataObj.AddedDate, resultObj.AddedDate);
                Assert.AreEqual(dataObj.Cover, resultObj.Cover);
            }
        }

        [DataTestMethod]
        [TestMethodCategory("GetChapters")]
        [TestMethodDelay()]
        [DataRow("https://readmanga.live/haven_t_you_heard__i_m_sakamoto", DisplayName = "'Haven't You Heard? I'm Sakamoto' chapters")]
        [DataRow("https://readmanga.live/legenda_o_serebrianom_klyke_akame", DisplayName = "'Legend of Silver Fang Akame' chapters")]
        [DataRow("https://readmanga.live/castle_town_dandelion", DisplayName = "'Castle Town Dandelion' chapters")]
        public override void GetChapters_ByUri_ShouldReturn_CorrectData(string url)
        {
            // arrange
            Uri uri = new Uri(url);

            IParser parser = GetNewParser;
            List<IChapter> data = new List<IChapter>();

            string dataPath = GetDataPath(ParserPath, "GetChapters", Path.GetFileName(url));

            if (File.Exists(dataPath))
            {
                using (var reader = new StreamReader(dataPath))
                {
                    string json = reader.ReadToEnd();
                    data = JsonConvert.DeserializeObject<List<IChapter>>(json, new FromJsonToDataConverter());
                }
            }

            // act
            var result = parser.GetChapters(uri).ToList();

            // assert
            CollectionAssert.AreEquivalent(data, result);

            foreach (IChapter resultObj in result)
            {
                IChapter dataObj = data.First(o => o.Url.Equals(resultObj.Url));

                Assert.AreEqual(dataObj.AddedDate, resultObj.AddedDate);
                Assert.AreEqual(dataObj.Cover, resultObj.Cover);
            }
        }

        [DataTestMethod]
        [TestMethodCategory("GetChapters")]
        [TestMethodDelay()]
        [DataRow("https://readmanga.live/haven_t_you_heard__i_m_sakamoto", DisplayName = "'Haven't You Heard? I'm Sakamoto' chapters")]
        [DataRow("https://readmanga.live/legenda_o_serebrianom_klyke_akame", DisplayName = "'Legend of Silver Fang Akame' chapters")]
        [DataRow("https://readmanga.live/castle_town_dandelion", DisplayName = "'Castle Town Dandelion' chapters")]
        public override void GetChapters_ByMangaObject_ShouldReturn_CorrectData(string url)
        {
            // arrange
            IMangaObject manga = new MangaObject(null, url);

            IParser parser = GetNewParser;
            List<IChapter> data = new List<IChapter>();

            string dataPath = GetDataPath(ParserPath, "GetChapters", Path.GetFileName(url));

            if (File.Exists(dataPath))
            {
                using (var reader = new StreamReader(dataPath))
                {
                    string json = reader.ReadToEnd();
                    data = JsonConvert.DeserializeObject<List<IChapter>>(json, new FromJsonToDataConverter());
                }
            }

            // act
            var result = parser.GetChapters(manga).ToList();

            // assert
            CollectionAssert.AreEquivalent(data, result);

            foreach (IChapter resultObj in result)
            {
                IChapter dataObj = data.First(o => o.Url.Equals(resultObj.Url));

                Assert.AreEqual(dataObj.AddedDate, resultObj.AddedDate);
                Assert.AreEqual(dataObj.Cover, resultObj.Cover);
            }
        }

        #endregion Sync

        #region Async

        [DataTestMethod]
        [TestMethodCategory("GetChapters", "Async")]
        [TestMethodDelay()]
        [DataRow("https://readmanga.live/haven_t_you_heard__i_m_sakamoto", DisplayName = "'Haven't You Heard? I'm Sakamoto' chapters")]
        [DataRow("https://readmanga.live/legenda_o_serebrianom_klyke_akame", DisplayName = "'Legend of Silver Fang Akame' chapters")]
        [DataRow("https://readmanga.live/castle_town_dandelion", DisplayName = "'Castle Town Dandelion' chapters")]
        public override async Task GetChaptersAsync_ByUrlString_ShouldReturn_CorrectData(string url)
        {
            // arrange
            IParser parser = GetNewParser;
            List<IChapter> data = new List<IChapter>();

            string dataPath = GetDataPath(ParserPath, "GetChapters", Path.GetFileName(url));

            if (File.Exists(dataPath))
            {
                using (var reader = new StreamReader(dataPath))
                {
                    string json = reader.ReadToEnd();
                    data = JsonConvert.DeserializeObject<List<IChapter>>(json, new FromJsonToDataConverter());
                }
            }

            // act
            var result = (await parser.GetChaptersAsync(url)).ToList();

            // assert
            CollectionAssert.AreEquivalent(data, result);

            foreach (IChapter resultObj in result)
            {
                IChapter dataObj = data.First(o => o.Url.Equals(resultObj.Url));

                Assert.AreEqual(dataObj.AddedDate, resultObj.AddedDate);
                Assert.AreEqual(dataObj.Cover, resultObj.Cover);
            }
        }

        [DataTestMethod]
        [TestMethodCategory("GetChapters", "Async")]
        [TestMethodDelay()]
        [DataRow("https://readmanga.live/haven_t_you_heard__i_m_sakamoto", DisplayName = "'Haven't You Heard? I'm Sakamoto' chapters")]
        [DataRow("https://readmanga.live/legenda_o_serebrianom_klyke_akame", DisplayName = "'Legend of Silver Fang Akame' chapters")]
        [DataRow("https://readmanga.live/castle_town_dandelion", DisplayName = "'Castle Town Dandelion' chapters")]
        public override async Task GetChaptersAsync_ByUri_ShouldReturn_CorrectData(string url)
        {
            // arrange
            Uri uri = new Uri(url);

            IParser parser = GetNewParser;
            List<IChapter> data = new List<IChapter>();

            string dataPath = GetDataPath(ParserPath, "GetChapters", Path.GetFileName(url));

            if (File.Exists(dataPath))
            {
                using (var reader = new StreamReader(dataPath))
                {
                    string json = reader.ReadToEnd();
                    data = JsonConvert.DeserializeObject<List<IChapter>>(json, new FromJsonToDataConverter());
                }
            }

            // act
            var result = (await parser.GetChaptersAsync(uri)).ToList();

            // assert
            CollectionAssert.AreEquivalent(data, result);

            foreach (IChapter resultObj in result)
            {
                IChapter dataObj = data.First(o => o.Url.Equals(resultObj.Url));

                Assert.AreEqual(dataObj.AddedDate, resultObj.AddedDate);
                Assert.AreEqual(dataObj.Cover, resultObj.Cover);
            }
        }

        [DataTestMethod]
        [TestMethodCategory("GetChapters", "Async")]
        [TestMethodDelay()]
        [DataRow("https://readmanga.live/haven_t_you_heard__i_m_sakamoto", DisplayName = "'Haven't You Heard? I'm Sakamoto' chapters")]
        [DataRow("https://readmanga.live/legenda_o_serebrianom_klyke_akame", DisplayName = "'Legend of Silver Fang Akame' chapters")]
        [DataRow("https://readmanga.live/castle_town_dandelion", DisplayName = "'Castle Town Dandelion' chapters")]
        public override async Task GetChaptersAsync_ByMangaObject_ShouldReturn_CorrectData(string url)
        {
            // arrange
            IMangaObject manga = new MangaObject(null, url);

            IParser parser = GetNewParser;
            List<IChapter> data = new List<IChapter>();

            string dataPath = GetDataPath(ParserPath, "GetChapters", Path.GetFileName(url));

            if (File.Exists(dataPath))
            {
                using (var reader = new StreamReader(dataPath))
                {
                    string json = reader.ReadToEnd();
                    data = JsonConvert.DeserializeObject<List<IChapter>>(json, new FromJsonToDataConverter());
                }
            }

            // act
            var result = (await parser.GetChaptersAsync(manga)).ToList();

            // assert
            CollectionAssert.AreEquivalent(data, result);

            foreach (IChapter resultObj in result)
            {
                IChapter dataObj = data.First(o => o.Url.Equals(resultObj.Url));

                Assert.AreEqual(dataObj.AddedDate, resultObj.AddedDate);
                Assert.AreEqual(dataObj.Cover, resultObj.Cover);
            }
        }

        #endregion Async

        #endregion GetChapters

        #region GetPages

        #region Sync

        [DataTestMethod]
        [TestMethodCategory("GetPages")]
        [TestMethodDelay()]
        [DataRow("https://readmanga.live/haven_t_you_heard__i_m_sakamoto/vol1/1", DisplayName = "'Haven't You Heard? I'm Sakamoto' ch1 pages")]
        [DataRow("https://readmanga.live/haven_t_you_heard__i_m_sakamoto/vol3/16", DisplayName = "'Haven't You Heard? I'm Sakamoto' ch16 pages")]
        [DataRow("https://readmanga.live/legenda_o_serebrianom_klyke_akame/vol1/1", DisplayName = "'Legend of Silver Fang Akame' ch1 pages")]
        [DataRow("https://readmanga.live/legenda_o_serebrianom_klyke_akame/vol1/7", DisplayName = "'Legend of Silver Fang Akame' ch7 pages")]
        [DataRow("https://readmanga.live/castle_town_dandelion/vol1/1?mtr=1", DisplayName = "'Castle Town Dandelion' ch1 pages")]
        [DataRow("https://readmanga.live/castle_town_dandelion/vol1/3?mtr=1", DisplayName = "'Castle Town Dandelion' ch3 pages")]
        public override void GetPages_ByUrlString_ShouldReturn_CorrectData(string url)
        {
            // arrange
            IParser parser = GetNewParser;
            List<IDataBase> data = new List<IDataBase>();

            string dataPath = GetDataPath(ParserPath, "GetPages", Uri.EscapeDataString(Path.GetRelativePath(parser.BaseUrl.OriginalString, url)));

            if (File.Exists(dataPath))
            {
                using (var reader = new StreamReader(dataPath))
                {
                    string json = reader.ReadToEnd();
                    data = JsonConvert.DeserializeObject<List<IDataBase>>(json, new FromJsonToDataConverter());
                }
            }

            // act
            var result = parser.GetPages(url).ToList();

            // assert
            Assert.AreEqual(data.Count, result.Count);

            for (int i = 0; i < result.Count; i++)
            {
                Assert.AreEqual(data[i].Url.LocalPath, result[i].Url.LocalPath);
                Assert.AreEqual(data[i].Value, result[i].Value);
            }
        }

        [DataTestMethod]
        [TestMethodCategory("GetPages")]
        [TestMethodDelay()]
        [DataRow("https://readmanga.live/haven_t_you_heard__i_m_sakamoto/vol1/1", DisplayName = "'Haven't You Heard? I'm Sakamoto' ch1 pages")]
        [DataRow("https://readmanga.live/haven_t_you_heard__i_m_sakamoto/vol3/16", DisplayName = "'Haven't You Heard? I'm Sakamoto' ch16 pages")]
        [DataRow("https://readmanga.live/legenda_o_serebrianom_klyke_akame/vol1/1", DisplayName = "'Legend of Silver Fang Akame' ch1 pages")]
        [DataRow("https://readmanga.live/legenda_o_serebrianom_klyke_akame/vol1/7", DisplayName = "'Legend of Silver Fang Akame' ch7 pages")]
        [DataRow("https://readmanga.live/castle_town_dandelion/vol1/1?mtr=1", DisplayName = "'Castle Town Dandelion' ch1 pages")]
        [DataRow("https://readmanga.live/castle_town_dandelion/vol1/3?mtr=1", DisplayName = "'Castle Town Dandelion' ch3 pages")]
        public override void GetPages_ByUri_ShouldReturn_CorrectData(string url)
        {
            // arrange
            Uri uri = new Uri(url);

            IParser parser = GetNewParser;
            List<IDataBase> data = new List<IDataBase>();

            string dataPath = GetDataPath(ParserPath, "GetPages", Uri.EscapeDataString(Path.GetRelativePath(parser.BaseUrl.OriginalString, url)));

            if (File.Exists(dataPath))
            {
                using (var reader = new StreamReader(dataPath))
                {
                    string json = reader.ReadToEnd();
                    data = JsonConvert.DeserializeObject<List<IDataBase>>(json, new FromJsonToDataConverter());
                }
            }

            // act
            var result = parser.GetPages(uri).ToList();

            // assert
            Assert.AreEqual(data.Count, result.Count);

            for (int i = 0; i < result.Count; i++)
            {
                Assert.AreEqual(data[i].Url.LocalPath, result[i].Url.LocalPath);
                Assert.AreEqual(data[i].Value, result[i].Value);
            }
        }

        [DataTestMethod]
        [TestMethodCategory("GetPages")]
        [TestMethodDelay()]
        [DataRow("https://readmanga.live/haven_t_you_heard__i_m_sakamoto/vol1/1", DisplayName = "'Haven't You Heard? I'm Sakamoto' ch1 pages")]
        [DataRow("https://readmanga.live/haven_t_you_heard__i_m_sakamoto/vol3/16", DisplayName = "'Haven't You Heard? I'm Sakamoto' ch16 pages")]
        [DataRow("https://readmanga.live/legenda_o_serebrianom_klyke_akame/vol1/1", DisplayName = "'Legend of Silver Fang Akame' ch1 pages")]
        [DataRow("https://readmanga.live/legenda_o_serebrianom_klyke_akame/vol1/7", DisplayName = "'Legend of Silver Fang Akame' ch7 pages")]
        [DataRow("https://readmanga.live/castle_town_dandelion/vol1/1?mtr=1", DisplayName = "'Castle Town Dandelion' ch1 pages")]
        [DataRow("https://readmanga.live/castle_town_dandelion/vol1/3?mtr=1", DisplayName = "'Castle Town Dandelion' ch3 pages")]
        public override void GetPages_ByChapter_ShouldReturn_CorrectData(string url)
        {
            // arrange
            IChapter chapter = new Chapter(null, url, DateTime.Now);

            IParser parser = GetNewParser;
            List<IDataBase> data = new List<IDataBase>();

            string dataPath = GetDataPath(ParserPath, "GetPages", Uri.EscapeDataString(Path.GetRelativePath(parser.BaseUrl.OriginalString, url)));

            if (File.Exists(dataPath))
            {
                using (var reader = new StreamReader(dataPath))
                {
                    string json = reader.ReadToEnd();
                    data = JsonConvert.DeserializeObject<List<IDataBase>>(json, new FromJsonToDataConverter());
                }
            }

            // act
            var result = parser.GetPages(chapter).ToList();

            // assert
            Assert.AreEqual(data.Count, result.Count);

            for (int i = 0; i < result.Count; i++)
            {
                Assert.AreEqual(data[i].Url.LocalPath, result[i].Url.LocalPath);
                Assert.AreEqual(data[i].Value, result[i].Value);
            }
        }

        #endregion Sync

        #region Async

        [DataTestMethod]
        [TestMethodCategory("GetPages", "Async")]
        [TestMethodDelay()]
        [DataRow("https://readmanga.live/haven_t_you_heard__i_m_sakamoto/vol1/1", DisplayName = "'Haven't You Heard? I'm Sakamoto' ch1 pages")]
        [DataRow("https://readmanga.live/haven_t_you_heard__i_m_sakamoto/vol3/16", DisplayName = "'Haven't You Heard? I'm Sakamoto' ch16 pages")]
        [DataRow("https://readmanga.live/legenda_o_serebrianom_klyke_akame/vol1/1", DisplayName = "'Legend of Silver Fang Akame' ch1 pages")]
        [DataRow("https://readmanga.live/legenda_o_serebrianom_klyke_akame/vol1/7", DisplayName = "'Legend of Silver Fang Akame' ch7 pages")]
        [DataRow("https://readmanga.live/castle_town_dandelion/vol1/1?mtr=1", DisplayName = "'Castle Town Dandelion' ch1 pages")]
        [DataRow("https://readmanga.live/castle_town_dandelion/vol1/3?mtr=1", DisplayName = "'Castle Town Dandelion' ch3 pages")]
        public override async Task GetPagesAsync_ByUrlString_ShouldReturn_CorrectData(string url)
        {
            // arrange
            IParser parser = GetNewParser;
            List<IDataBase> data = new List<IDataBase>();

            string dataPath = GetDataPath(ParserPath, "GetPages", Uri.EscapeDataString(Path.GetRelativePath(parser.BaseUrl.OriginalString, url)));

            if (File.Exists(dataPath))
            {
                using (var reader = new StreamReader(dataPath))
                {
                    string json = reader.ReadToEnd();
                    data = JsonConvert.DeserializeObject<List<IDataBase>>(json, new FromJsonToDataConverter());
                }
            }

            // act
            var result = (await parser.GetPagesAsync(url)).ToList();

            // assert
            Assert.AreEqual(data.Count, result.Count);

            for (int i = 0; i < result.Count; i++)
            {
                Assert.AreEqual(data[i].Url.LocalPath, result[i].Url.LocalPath);
                Assert.AreEqual(data[i].Value, result[i].Value);
            }
        }

        [DataTestMethod]
        [TestMethodCategory("GetPages", "Async")]
        [TestMethodDelay()]
        [DataRow("https://readmanga.live/haven_t_you_heard__i_m_sakamoto/vol1/1", DisplayName = "'Haven't You Heard? I'm Sakamoto' ch1 pages")]
        [DataRow("https://readmanga.live/haven_t_you_heard__i_m_sakamoto/vol3/16", DisplayName = "'Haven't You Heard? I'm Sakamoto' ch16 pages")]
        [DataRow("https://readmanga.live/legenda_o_serebrianom_klyke_akame/vol1/1", DisplayName = "'Legend of Silver Fang Akame' ch1 pages")]
        [DataRow("https://readmanga.live/legenda_o_serebrianom_klyke_akame/vol1/7", DisplayName = "'Legend of Silver Fang Akame' ch7 pages")]
        [DataRow("https://readmanga.live/castle_town_dandelion/vol1/1?mtr=1", DisplayName = "'Castle Town Dandelion' ch1 pages")]
        [DataRow("https://readmanga.live/castle_town_dandelion/vol1/3?mtr=1", DisplayName = "'Castle Town Dandelion' ch3 pages")]
        public override async Task GetPagesAsync_ByUri_ShouldReturn_CorrectData(string url)
        {
            // arrange
            Uri uri = new Uri(url);

            IParser parser = GetNewParser;
            List<IDataBase> data = new List<IDataBase>();

            string dataPath = GetDataPath(ParserPath, "GetPages", Uri.EscapeDataString(Path.GetRelativePath(parser.BaseUrl.OriginalString, url)));

            if (File.Exists(dataPath))
            {
                using (var reader = new StreamReader(dataPath))
                {
                    string json = reader.ReadToEnd();
                    data = JsonConvert.DeserializeObject<List<IDataBase>>(json, new FromJsonToDataConverter());
                }
            }

            // act
            var result = (await parser.GetPagesAsync(uri)).ToList();

            // assert
            Assert.AreEqual(data.Count, result.Count);

            for (int i = 0; i < result.Count; i++)
            {
                Assert.AreEqual(data[i].Url.LocalPath, result[i].Url.LocalPath);
                Assert.AreEqual(data[i].Value, result[i].Value);
            }
        }

        [DataTestMethod]
        [TestMethodCategory("GetPages", "Async")]
        [TestMethodDelay()]
        [DataRow("https://readmanga.live/haven_t_you_heard__i_m_sakamoto/vol1/1", DisplayName = "'Haven't You Heard? I'm Sakamoto' ch1 pages")]
        [DataRow("https://readmanga.live/haven_t_you_heard__i_m_sakamoto/vol3/16", DisplayName = "'Haven't You Heard? I'm Sakamoto' ch16 pages")]
        [DataRow("https://readmanga.live/legenda_o_serebrianom_klyke_akame/vol1/1", DisplayName = "'Legend of Silver Fang Akame' ch1 pages")]
        [DataRow("https://readmanga.live/legenda_o_serebrianom_klyke_akame/vol1/7", DisplayName = "'Legend of Silver Fang Akame' ch7 pages")]
        [DataRow("https://readmanga.live/castle_town_dandelion/vol1/1?mtr=1", DisplayName = "'Castle Town Dandelion' ch1 pages")]
        [DataRow("https://readmanga.live/castle_town_dandelion/vol1/3?mtr=1", DisplayName = "'Castle Town Dandelion' ch3 pages")]
        public override async Task GetPagesAsync_ByChapter_ShouldReturn_CorrectData(string url)
        {
            // arrange
            IChapter chapter = new Chapter(null, url, DateTime.Now);

            IParser parser = GetNewParser;
            List<IDataBase> data = new List<IDataBase>();

            string dataPath = GetDataPath(ParserPath, "GetPages", Uri.EscapeDataString(Path.GetRelativePath(parser.BaseUrl.OriginalString, url)));

            if (File.Exists(dataPath))
            {
                using (var reader = new StreamReader(dataPath))
                {
                    string json = reader.ReadToEnd();
                    data = JsonConvert.DeserializeObject<List<IDataBase>>(json, new FromJsonToDataConverter());
                }
            }

            // act
            var result = (await parser.GetPagesAsync(chapter)).ToList();

            // assert
            Assert.AreEqual(data.Count, result.Count);

            for (int i = 0; i < result.Count; i++)
            {
                Assert.AreEqual(data[i].Url.LocalPath, result[i].Url.LocalPath);
                Assert.AreEqual(data[i].Value, result[i].Value);
            }
        }

        #endregion Async

        #endregion GetPages
    }
}