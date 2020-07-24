using MangaParser.Core.Exceptions;
using MangaParser.Core.Interfaces;
using MangaParser.Core.Models;
using MangaParser.Parsers.HtmlWebParsers.ReadManga;
using MangaParser.Parsers.Tests.Converters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MangaParser.Parsers.Tests.HtmlWebParsers.ReadManga
{
    [TestClass]
    [TestCategory("Parser")]
    [TestCategory("ReadManga")]
    public class ReadMangaParserTests
    {
        protected const string parserPath = "ReadManga";
        protected const string dataPath = "Data";

        protected virtual string GetDataPath(string parserPath, string methodPath, string name, string extension = ".json")
        {
            return Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", dataPath ?? string.Empty, parserPath ?? string.Empty, methodPath ?? string.Empty, name + extension ?? string.Empty));
        }

        [TestCleanup]
        public void TestCleanup()
        {
            // After passing a test, we wait to avoid HTTP flooding and getting caught by DDoS detection.
            Thread.Sleep(1500);
        }

        #region Search

        #region Sync

        [DataTestMethod]
        [TestCategory("Search")]
        [DataRow("sakamoto", DisplayName = "'sakamoto' query")]
        [DataRow("akame", DisplayName = "'akame' query")]
        [DataRow("Castle Town Dandelion", DisplayName = "Multiple word query")]
        [DataRow("asdfasedfqewqfeqwfq", DisplayName = "Rubbish query")]
        [DataRow("", DisplayName = "Empty query")]
        [DataRow(null, DisplayName = "Null query")]
        public void SearchManga_WithQuery_ShouldReturn_CorrectData(string query)
        {
            // arrange
            ReadMangaParser parser = new ReadMangaParser();
            List<IMangaObject> data = new List<IMangaObject>();

            string dataPath = GetDataPath(parserPath, "Search", query);

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
        [TestCategory("Search")]
        [TestCategory("Async")]
        [DataRow("sakamoto", DisplayName = "'sakamoto' query")]
        [DataRow("akame", DisplayName = "'akame' query")]
        [DataRow("Castle Town Dandelion", DisplayName = "Multiple word query")]
        [DataRow("asdfasedfqewqfeqwfq", DisplayName = "Rubbish query")]
        [DataRow("", DisplayName = "Empty query")]
        [DataRow(null, DisplayName = "Null query")]
        public async Task SearchMangaAsync_WithQuery_ShouldReturn_CorrectData(string query)
        {
            // arrange
            ReadMangaParser parser = new ReadMangaParser();
            List<IMangaObject> data = new List<IMangaObject>();

            string dataPath = GetDataPath(parserPath, "Search", query);

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
        [TestCategory("GetManga")]
        [DataRow("https://readmanga.live/haven_t_you_heard__i_m_sakamoto", DisplayName = "'Haven't You Heard? I'm Sakamoto' url")]
        [DataRow("https://readmanga.live/legenda_o_serebrianom_klyke_akame", DisplayName = "'Legend of Silver Fang Akame' url")]
        [DataRow("https://readmanga.live/castle_town_dandelion", DisplayName = "'Castle Town Dandelion' url")]
        [DataRow("https://readmanga.live/one_punch_man__A1bc88e", DisplayName = "'One Punch-Man' url")]
        public void GetManga_ByUrlString_ShouldReturn_CorrectData(string url)
        {
            // arrange
            ReadMangaParser parser = new ReadMangaParser();
            IMangaObject data = null;

            string dataPath = GetDataPath(parserPath, "GetManga", Path.GetFileName(url));

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
        [TestCategory("GetManga")]
        [DataRow("https://readmanga.live/haven_t_you_heard__i_m_sakamoto", DisplayName = "'Haven't You Heard? I'm Sakamoto' url")]
        [DataRow("https://readmanga.live/legenda_o_serebrianom_klyke_akame", DisplayName = "'Legend of Silver Fang Akame' url")]
        [DataRow("https://readmanga.live/castle_town_dandelion", DisplayName = "'Castle Town Dandelion' url")]
        [DataRow("https://readmanga.live/one_punch_man__A1bc88e", DisplayName = "'One Punch-Man' url")]
        public void GetManga_ByUri_ShouldReturn_CorrectData(string url)
        {
            // arrange
            Uri uri = new Uri(url);

            ReadMangaParser parser = new ReadMangaParser();
            IMangaObject data = null;

            string dataPath = GetDataPath(parserPath, "GetManga", Path.GetFileName(url));

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
        [TestCategory("GetManga")]
        [DataRow("https://readmanga.live/haven_t_you_heard__i_m_sakamoto", DisplayName = "'Haven't You Heard? I'm Sakamoto' url")]
        [DataRow("https://readmanga.live/legenda_o_serebrianom_klyke_akame", DisplayName = "'Legend of Silver Fang Akame' url")]
        [DataRow("https://readmanga.live/castle_town_dandelion", DisplayName = "'Castle Town Dandelion' url")]
        [DataRow("https://readmanga.live/one_punch_man__A1bc88e", DisplayName = "'One Punch-Man' url")]
        public void GetManga_ByMangaObject_ShouldReturn_CorrectData(string url)
        {
            // arrange
            IMangaObject manga = new MangaObject(null, url);

            ReadMangaParser parser = new ReadMangaParser();
            IMangaObject data = null;

            string dataPath = GetDataPath(parserPath, "GetManga", Path.GetFileName(url));

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

        [DataTestMethod]
        [TestCategory("GetManga")]
        [DataRow("asdfasedfqewqfeqwfq", DisplayName = "Rubbish url")]
        [DataRow("", DisplayName = "Empty url")]
        [DataRow(null, DisplayName = "Null url")]
        public void GetManga_ByWrongUrlString_ShouldThrow_UriFormatException(string url)
        {
            // arrange
            ReadMangaParser parser = new ReadMangaParser();
            Action action = new Action(() => parser.GetManga(url));

            // act
            // assert
            Assert.ThrowsException<UriFormatException>(action);
        }

        [DataTestMethod]
        [TestCategory("GetManga")]
        [DataRow("asdfasedfqewqfeqwfq", DisplayName = "Rubbish url")]
        [DataRow("", DisplayName = "Empty url")]
        [DataRow(null, DisplayName = "Null url")]
        public void GetManga_ByWrongMangaObject_ShouldThrow_ArgumentNullException(string url)
        {
            // arrange
            IMangaObject manga = new MangaObject(null, url);

            ReadMangaParser parser = new ReadMangaParser();
            Action action = new Action(() => parser.GetManga(manga));

            // act
            // assert
            Assert.ThrowsException<ArgumentNullException>(action);
        }

        [DataTestMethod]
        [TestCategory("GetManga")]
        [DataRow("http://example.com/", DisplayName = "Wrong host url")]
        public void GetManga_ByWrongUrlStringHost_ShouldThrow_BaseHostNotMatchException(string url)
        {
            // arrange
            ReadMangaParser parser = new ReadMangaParser();
            Action action = new Action(() => parser.GetManga(url));

            // act
            // assert
            Assert.ThrowsException<BaseHostNotMatchException>(action);
        }

        [DataTestMethod]
        [TestCategory("GetManga")]
        [DataRow("http://example.com/", DisplayName = "Wrong host url")]
        public void GetManga_ByWrongUriHost_ShouldThrow_BaseHostNotMatchException(string url)
        {
            // arrange
            Uri uri = new Uri(url);

            ReadMangaParser parser = new ReadMangaParser();
            Action action = new Action(() => parser.GetManga(uri));

            // act
            // assert
            Assert.ThrowsException<BaseHostNotMatchException>(action);
        }

        [DataTestMethod]
        [TestCategory("GetManga")]
        [DataRow("http://example.com/", DisplayName = "Wrong host url")]
        public void GetManga_ByWrongMangaObjectHost_ShouldThrow_BaseHostNotMatchException(string url)
        {
            // arrange
            IMangaObject manga = new MangaObject(null, url);

            ReadMangaParser parser = new ReadMangaParser();
            Action action = new Action(() => parser.GetManga(manga));

            // act
            // assert
            Assert.ThrowsException<BaseHostNotMatchException>(action);
        }

        #endregion Sync

        #region Async

        [DataTestMethod]
        [TestCategory("GetManga")]
        [TestCategory("Async")]
        [DataRow("https://readmanga.live/haven_t_you_heard__i_m_sakamoto", DisplayName = "'Haven't You Heard? I'm Sakamoto' url")]
        [DataRow("https://readmanga.live/legenda_o_serebrianom_klyke_akame", DisplayName = "'Legend of Silver Fang Akame' url")]
        [DataRow("https://readmanga.live/castle_town_dandelion", DisplayName = "'Castle Town Dandelion' url")]
        [DataRow("https://readmanga.live/one_punch_man__A1bc88e", DisplayName = "'One Punch-Man' url")]
        public async Task GetMangaAsync_ByUrlString_ShouldReturn_CorrectData(string url)
        {
            // arrange
            ReadMangaParser parser = new ReadMangaParser();
            IMangaObject data = null;

            string dataPath = GetDataPath(parserPath, "GetManga", Path.GetFileName(url));

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
        [TestCategory("GetManga")]
        [TestCategory("Async")]
        [DataRow("https://readmanga.live/haven_t_you_heard__i_m_sakamoto", DisplayName = "'Haven't You Heard? I'm Sakamoto' url")]
        [DataRow("https://readmanga.live/legenda_o_serebrianom_klyke_akame", DisplayName = "'Legend of Silver Fang Akame' url")]
        [DataRow("https://readmanga.live/castle_town_dandelion", DisplayName = "'Castle Town Dandelion' url")]
        [DataRow("https://readmanga.live/one_punch_man__A1bc88e", DisplayName = "'One Punch-Man' url")]
        public async Task GetMangaAsync_ByUri_ShouldReturn_CorrectData(string url)
        {
            // arrange
            Uri uri = new Uri(url);

            ReadMangaParser parser = new ReadMangaParser();
            IMangaObject data = null;

            string dataPath = GetDataPath(parserPath, "GetManga", Path.GetFileName(url));

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
        [TestCategory("GetManga")]
        [TestCategory("Async")]
        [DataRow("https://readmanga.live/haven_t_you_heard__i_m_sakamoto", DisplayName = "'Haven't You Heard? I'm Sakamoto' url")]
        [DataRow("https://readmanga.live/legenda_o_serebrianom_klyke_akame", DisplayName = "'Legend of Silver Fang Akame' url")]
        [DataRow("https://readmanga.live/castle_town_dandelion", DisplayName = "'Castle Town Dandelion' url")]
        [DataRow("https://readmanga.live/one_punch_man__A1bc88e", DisplayName = "'One Punch-Man' url")]
        public async Task GetMangaAsync_ByMangaObject_ShouldReturn_CorrectData(string url)
        {
            // arrange
            IMangaObject manga = new MangaObject(null, url);

            ReadMangaParser parser = new ReadMangaParser();
            IMangaObject data = null;

            string dataPath = GetDataPath(parserPath, "GetManga", Path.GetFileName(url));

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

        [DataTestMethod]
        [TestCategory("GetManga")]
        [TestCategory("Async")]
        [DataRow("asdfasedfqewqfeqwfq", DisplayName = "Rubbish url")]
        [DataRow("", DisplayName = "Empty url")]
        [DataRow(null, DisplayName = "Null url")]
        public async Task GetMangaAsync_ByWrongUrlString_ShouldThrow_UriFormatException(string url)
        {
            // arrange
            ReadMangaParser parser = new ReadMangaParser();
            Task<IMangaObject> task()
            {
                return parser.GetMangaAsync(url);
            }

            // act
            // assert
            await Assert.ThrowsExceptionAsync<UriFormatException>(task);
        }

        [DataTestMethod]
        [TestCategory("GetManga")]
        [TestCategory("Async")]
        [DataRow("asdfasedfqewqfeqwfq", DisplayName = "Rubbish url")]
        [DataRow("", DisplayName = "Empty url")]
        [DataRow(null, DisplayName = "Null url")]
        public async Task GetMangaAsync_ByWrongMangaObject_ShouldThrow_ArgumentNullException(string url)
        {
            // arrange
            IMangaObject manga = new MangaObject(null, url);

            ReadMangaParser parser = new ReadMangaParser();
            Task<IMangaObject> task()
            {
                return parser.GetMangaAsync(manga);
            }

            // act
            // assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(task);
        }

        [DataTestMethod]
        [TestCategory("GetManga")]
        [TestCategory("Async")]
        [DataRow("http://example.com/", DisplayName = "Wrong host url")]
        public async Task GetMangaAsync_ByWrongUrlStringHost_ShouldThrow_BaseHostNotMatchException(string url)
        {
            // arrange
            ReadMangaParser parser = new ReadMangaParser();
            Task<IMangaObject> task()
            {
                return parser.GetMangaAsync(url);
            }

            // act
            // assert
            await Assert.ThrowsExceptionAsync<BaseHostNotMatchException>(task);
        }

        [DataTestMethod]
        [TestCategory("GetManga")]
        [TestCategory("Async")]
        [DataRow("http://example.com/", DisplayName = "Wrong host url")]
        public async Task GetMangaAsync_ByWrongUriHost_ShouldThrow_BaseHostNotMatchException(string url)
        {
            // arrange
            Uri uri = new Uri(url);

            ReadMangaParser parser = new ReadMangaParser();
            Task<IMangaObject> task()
            {
                return parser.GetMangaAsync(uri);
            }

            // act
            // assert
            await Assert.ThrowsExceptionAsync<BaseHostNotMatchException>(task);
        }

        [DataTestMethod]
        [TestCategory("GetManga")]
        [TestCategory("Async")]
        [DataRow("http://example.com/", DisplayName = "Wrong host url")]
        public async Task GetMangaAsync_ByWrongMangaObjectHost_ShouldThrow_BaseHostNotMatchException(string url)
        {
            // arrange
            IMangaObject manga = new MangaObject(null, url);

            ReadMangaParser parser = new ReadMangaParser();
            Task<IMangaObject> task()
            {
                return parser.GetMangaAsync(manga);
            }

            // act
            // assert
            await Assert.ThrowsExceptionAsync<BaseHostNotMatchException>(task);
        }

        #endregion Async

        #endregion GetManga

        #region GetChapters

        #region Sync

        [DataTestMethod]
        [TestCategory("GetChapters")]
        [DataRow("https://readmanga.live/haven_t_you_heard__i_m_sakamoto", DisplayName = "'Haven't You Heard? I'm Sakamoto' chapters")]
        [DataRow("https://readmanga.live/legenda_o_serebrianom_klyke_akame", DisplayName = "'Legend of Silver Fang Akame' chapters")]
        [DataRow("https://readmanga.live/castle_town_dandelion", DisplayName = "'Castle Town Dandelion' chapters")]
        public void GetChapters_ByUrlString_ShouldReturn_CorrectData(string url)
        {
            // arrange
            ReadMangaParser parser = new ReadMangaParser();
            List<IChapter> data = new List<IChapter>();

            string dataPath = GetDataPath(parserPath, "GetChapters", Path.GetFileName(url));

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
        [TestCategory("GetChapters")]
        [DataRow("https://readmanga.live/haven_t_you_heard__i_m_sakamoto", DisplayName = "'Haven't You Heard? I'm Sakamoto' chapters")]
        [DataRow("https://readmanga.live/legenda_o_serebrianom_klyke_akame", DisplayName = "'Legend of Silver Fang Akame' chapters")]
        [DataRow("https://readmanga.live/castle_town_dandelion", DisplayName = "'Castle Town Dandelion' chapters")]
        public void GetChapters_ByUri_ShouldReturn_CorrectData(string url)
        {
            // arrange
            Uri uri = new Uri(url);

            ReadMangaParser parser = new ReadMangaParser();
            List<IChapter> data = new List<IChapter>();

            string dataPath = GetDataPath(parserPath, "GetChapters", Path.GetFileName(url));

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
        [TestCategory("GetChapters")]
        [DataRow("https://readmanga.live/haven_t_you_heard__i_m_sakamoto", DisplayName = "'Haven't You Heard? I'm Sakamoto' chapters")]
        [DataRow("https://readmanga.live/legenda_o_serebrianom_klyke_akame", DisplayName = "'Legend of Silver Fang Akame' chapters")]
        [DataRow("https://readmanga.live/castle_town_dandelion", DisplayName = "'Castle Town Dandelion' chapters")]
        public void GetChapters_ByMangaObject_ShouldReturn_CorrectData(string url)
        {
            // arrange
            IMangaObject manga = new MangaObject(null, url);

            ReadMangaParser parser = new ReadMangaParser();
            List<IChapter> data = new List<IChapter>();

            string dataPath = GetDataPath(parserPath, "GetChapters", Path.GetFileName(url));

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

        [DataTestMethod]
        [TestCategory("GetChapters")]
        [DataRow("asdfasedfqewqfeqwfq", DisplayName = "Rubbish url")]
        [DataRow("", DisplayName = "Empty url")]
        [DataRow(null, DisplayName = "Null url")]
        public void GetChapters_ByWrongUrlString_ShouldThrow_UriFormatException(string url)
        {
            // arrange
            ReadMangaParser parser = new ReadMangaParser();
            Action action = new Action(() => parser.GetChapters(url));

            // act
            // assert
            Assert.ThrowsException<UriFormatException>(action);
        }

        [DataTestMethod]
        [TestCategory("GetChapters")]
        [DataRow("asdfasedfqewqfeqwfq", DisplayName = "Rubbish url")]
        [DataRow("", DisplayName = "Empty url")]
        [DataRow(null, DisplayName = "Null url")]
        public void GetChapters_ByWrongMangaObjecttUrl_ShouldThrow_ArgumentNullException(string url)
        {
            // arrange
            IMangaObject manga = new MangaObject(null, url);

            ReadMangaParser parser = new ReadMangaParser();
            Action action = new Action(() => parser.GetChapters(manga));

            // act
            // assert
            Assert.ThrowsException<ArgumentNullException>(action);
        }

        [DataTestMethod]
        [TestCategory("GetChapters")]
        [DataRow("http://example.com/", DisplayName = "Wrong host url")]
        public void GetChapters_ByWrongUrlStringHost_ShouldThrow_BaseHostNotMatchException(string url)
        {
            // arrange
            ReadMangaParser parser = new ReadMangaParser();
            Action action = new Action(() => parser.GetChapters(url));

            // act
            // assert
            Assert.ThrowsException<BaseHostNotMatchException>(action);
        }

        [DataTestMethod]
        [TestCategory("GetChapters")]
        [DataRow("http://example.com/", DisplayName = "Wrong host url")]
        public void GetChapters_ByWrongUriHost_ShouldThrow_BaseHostNotMatchException(string url)
        {
            // arrange
            Uri uri = new Uri(url);

            ReadMangaParser parser = new ReadMangaParser();
            Action action = new Action(() => parser.GetChapters(uri));

            // act
            // assert
            Assert.ThrowsException<BaseHostNotMatchException>(action);
        }

        [DataTestMethod]
        [TestCategory("GetChapters")]
        [DataRow("http://example.com/", DisplayName = "Wrong host url")]
        public void GetChapters_ByWrongMangaObjectUrltHost_ShouldThrow_BaseHostNotMatchException(string url)
        {
            // arrange
            IMangaObject manga = new MangaObject(null, url);

            ReadMangaParser parser = new ReadMangaParser();
            Action action = new Action(() => parser.GetChapters(manga));

            // act
            // assert
            Assert.ThrowsException<BaseHostNotMatchException>(action);
        }

        #endregion Sync

        #region Async

        [DataTestMethod]
        [TestCategory("GetChapters")]
        [TestCategory("Async")]
        [DataRow("https://readmanga.live/haven_t_you_heard__i_m_sakamoto", DisplayName = "'Haven't You Heard? I'm Sakamoto' chapters")]
        [DataRow("https://readmanga.live/legenda_o_serebrianom_klyke_akame", DisplayName = "'Legend of Silver Fang Akame' chapters")]
        [DataRow("https://readmanga.live/castle_town_dandelion", DisplayName = "'Castle Town Dandelion' chapters")]
        public async Task GetChaptersAsync_ByUrlString_ShouldReturn_CorrectData(string url)
        {
            // arrange
            ReadMangaParser parser = new ReadMangaParser();
            List<IChapter> data = new List<IChapter>();

            string dataPath = GetDataPath(parserPath, "GetChapters", Path.GetFileName(url));

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
        [TestCategory("GetChapters")]
        [TestCategory("Async")]
        [DataRow("https://readmanga.live/haven_t_you_heard__i_m_sakamoto", DisplayName = "'Haven't You Heard? I'm Sakamoto' chapters")]
        [DataRow("https://readmanga.live/legenda_o_serebrianom_klyke_akame", DisplayName = "'Legend of Silver Fang Akame' chapters")]
        [DataRow("https://readmanga.live/castle_town_dandelion", DisplayName = "'Castle Town Dandelion' chapters")]
        public async Task GetChaptersAsync_ByUri_ShouldReturn_CorrectData(string url)
        {
            // arrange
            Uri uri = new Uri(url);

            ReadMangaParser parser = new ReadMangaParser();
            List<IChapter> data = new List<IChapter>();

            string dataPath = GetDataPath(parserPath, "GetChapters", Path.GetFileName(url));

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
        [TestCategory("GetChapters")]
        [TestCategory("Async")]
        [DataRow("https://readmanga.live/haven_t_you_heard__i_m_sakamoto", DisplayName = "'Haven't You Heard? I'm Sakamoto' chapters")]
        [DataRow("https://readmanga.live/legenda_o_serebrianom_klyke_akame", DisplayName = "'Legend of Silver Fang Akame' chapters")]
        [DataRow("https://readmanga.live/castle_town_dandelion", DisplayName = "'Castle Town Dandelion' chapters")]
        public async Task GetChaptersAsync_ByMangaObject_ShouldReturn_CorrectData(string url)
        {
            // arrange
            IMangaObject manga = new MangaObject(null, url);

            ReadMangaParser parser = new ReadMangaParser();
            List<IChapter> data = new List<IChapter>();

            string dataPath = GetDataPath(parserPath, "GetChapters", Path.GetFileName(url));

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

        [DataTestMethod]
        [TestCategory("GetChapters")]
        [TestCategory("Async")]
        [DataRow("asdfasedfqewqfeqwfq", DisplayName = "Rubbish url")]
        [DataRow("", DisplayName = "Empty url")]
        [DataRow(null, DisplayName = "Null url")]
        public async Task GetChaptersAsync_ByWrongUrlString_ShouldThrow_UriFormatException(string url)
        {
            // arrange
            ReadMangaParser parser = new ReadMangaParser();

            Task<IEnumerable<IChapter>> task()
            {
                return parser.GetChaptersAsync(url);
            }

            // act
            // assert
            await Assert.ThrowsExceptionAsync<UriFormatException>(task);
        }

        [DataTestMethod]
        [TestCategory("GetChapters")]
        [TestCategory("Async")]
        [DataRow("asdfasedfqewqfeqwfq", DisplayName = "Rubbish url")]
        [DataRow("", DisplayName = "Empty url")]
        [DataRow(null, DisplayName = "Null url")]
        public async Task GetChaptersAsync_ByWrongMangaObjectUrl_ShouldThrow_ArgumentNullException(string url)
        {
            // arrange
            IMangaObject manga = new MangaObject(null, url);

            ReadMangaParser parser = new ReadMangaParser();

            Task<IEnumerable<IChapter>> task()
            {
                return parser.GetChaptersAsync(manga);
            }

            // act
            // assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(task);
        }

        [DataTestMethod]
        [TestCategory("GetChapters")]
        [TestCategory("Async")]
        [DataRow("http://example.com/", DisplayName = "Wrong host url")]
        public async Task GetChaptersAsync_ByWrongUrlStringHost_ShouldThrow_BaseHostNotMatchException(string url)
        {
            // arrange
            ReadMangaParser parser = new ReadMangaParser();

            Task<IEnumerable<IChapter>> task()
            {
                return parser.GetChaptersAsync(url);
            }

            // act
            // assert
            await Assert.ThrowsExceptionAsync<BaseHostNotMatchException>(task);
        }

        [DataTestMethod]
        [TestCategory("GetChapters")]
        [TestCategory("Async")]
        [DataRow("http://example.com/", DisplayName = "Wrong host url")]
        public async Task GetChaptersAsync_ByWrongUriHost_ShouldThrow_BaseHostNotMatchException(string url)
        {
            // arrange
            Uri uri = new Uri(url);

            ReadMangaParser parser = new ReadMangaParser();

            Task<IEnumerable<IChapter>> task()
            {
                return parser.GetChaptersAsync(uri);
            }

            // act
            // assert
            await Assert.ThrowsExceptionAsync<BaseHostNotMatchException>(task);
        }

        [DataTestMethod]
        [TestCategory("GetChapters")]
        [TestCategory("Async")]
        [DataRow("http://example.com/", DisplayName = "Wrong host url")]
        public async Task GetChaptersAsync_ByWrongMangaObjectUrltHost_ShouldThrow_BaseHostNotMatchException(string url)
        {
            // arrange
            IMangaObject manga = new MangaObject(null, url);

            ReadMangaParser parser = new ReadMangaParser();

            Task<IEnumerable<IChapter>> task()
            {
                return parser.GetChaptersAsync(manga);
            }

            // act
            // assert
            await Assert.ThrowsExceptionAsync<BaseHostNotMatchException>(task);
        }

        #endregion Async

        #endregion GetChapters

        #region GetPages

        #region Sync

        [DataTestMethod]
        [TestCategory("GetPages")]
        [DataRow("https://readmanga.live/haven_t_you_heard__i_m_sakamoto/vol1/1", DisplayName = "'Haven't You Heard? I'm Sakamoto' ch1 pages")]
        [DataRow("https://readmanga.live/haven_t_you_heard__i_m_sakamoto/vol3/16", DisplayName = "'Haven't You Heard? I'm Sakamoto' ch16 pages")]
        [DataRow("https://readmanga.live/legenda_o_serebrianom_klyke_akame/vol1/1", DisplayName = "'Legend of Silver Fang Akame' ch1 pages")]
        [DataRow("https://readmanga.live/legenda_o_serebrianom_klyke_akame/vol1/7", DisplayName = "'Legend of Silver Fang Akame' ch7 pages")]
        [DataRow("https://readmanga.live/castle_town_dandelion/vol1/1?mtr=1", DisplayName = "'Castle Town Dandelion' ch1 pages")]
        [DataRow("https://readmanga.live/castle_town_dandelion/vol1/3?mtr=1", DisplayName = "'Castle Town Dandelion' ch3 pages")]
        public void GetPages_ByUrlString_ShouldReturn_CorrectData(string url)
        {
            // arrange
            ReadMangaParser parser = new ReadMangaParser();
            List<IDataBase> data = new List<IDataBase>();

            string dataPath = GetDataPath(parserPath, "GetPages", Uri.EscapeDataString(Path.GetRelativePath(parser.BaseUrl.OriginalString, url)));

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
                Assert.AreEqual(data[i].Url.GetLeftPart(UriPartial.Path), result[i].Url.GetLeftPart(UriPartial.Path));
                Assert.AreEqual(data[i].Value, result[i].Value);
            }
        }

        [DataTestMethod]
        [TestCategory("GetPages")]
        [DataRow("https://readmanga.live/haven_t_you_heard__i_m_sakamoto/vol1/1", DisplayName = "'Haven't You Heard? I'm Sakamoto' ch1 pages")]
        [DataRow("https://readmanga.live/haven_t_you_heard__i_m_sakamoto/vol3/16", DisplayName = "'Haven't You Heard? I'm Sakamoto' ch16 pages")]
        [DataRow("https://readmanga.live/legenda_o_serebrianom_klyke_akame/vol1/1", DisplayName = "'Legend of Silver Fang Akame' ch1 pages")]
        [DataRow("https://readmanga.live/legenda_o_serebrianom_klyke_akame/vol1/7", DisplayName = "'Legend of Silver Fang Akame' ch7 pages")]
        [DataRow("https://readmanga.live/castle_town_dandelion/vol1/1?mtr=1", DisplayName = "'Castle Town Dandelion' ch1 pages")]
        [DataRow("https://readmanga.live/castle_town_dandelion/vol1/3?mtr=1", DisplayName = "'Castle Town Dandelion' ch3 pages")]
        public void GetPages_ByUri_ShouldReturn_CorrectData(string url)
        {
            // arrange
            Uri uri = new Uri(url);

            ReadMangaParser parser = new ReadMangaParser();
            List<IDataBase> data = new List<IDataBase>();

            string dataPath = GetDataPath(parserPath, "GetPages", Uri.EscapeDataString(Path.GetRelativePath(parser.BaseUrl.OriginalString, url)));

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
                Assert.AreEqual(data[i].Url.GetLeftPart(UriPartial.Path), result[i].Url.GetLeftPart(UriPartial.Path));
                Assert.AreEqual(data[i].Value, result[i].Value);
            }
        }

        [DataTestMethod]
        [TestCategory("GetPages")]
        [DataRow("https://readmanga.live/haven_t_you_heard__i_m_sakamoto/vol1/1", DisplayName = "'Haven't You Heard? I'm Sakamoto' ch1 pages")]
        [DataRow("https://readmanga.live/haven_t_you_heard__i_m_sakamoto/vol3/16", DisplayName = "'Haven't You Heard? I'm Sakamoto' ch16 pages")]
        [DataRow("https://readmanga.live/legenda_o_serebrianom_klyke_akame/vol1/1", DisplayName = "'Legend of Silver Fang Akame' ch1 pages")]
        [DataRow("https://readmanga.live/legenda_o_serebrianom_klyke_akame/vol1/7", DisplayName = "'Legend of Silver Fang Akame' ch7 pages")]
        [DataRow("https://readmanga.live/castle_town_dandelion/vol1/1?mtr=1", DisplayName = "'Castle Town Dandelion' ch1 pages")]
        [DataRow("https://readmanga.live/castle_town_dandelion/vol1/3?mtr=1", DisplayName = "'Castle Town Dandelion' ch3 pages")]
        public void GetPages_ByChapter_ShouldReturn_CorrectData(string url)
        {
            // arrange
            IChapter chapter = new Chapter(null, url, DateTime.Now);

            ReadMangaParser parser = new ReadMangaParser();
            List<IDataBase> data = new List<IDataBase>();

            string dataPath = GetDataPath(parserPath, "GetPages", Uri.EscapeDataString(Path.GetRelativePath(parser.BaseUrl.OriginalString, url)));

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
                Assert.AreEqual(data[i].Url.GetLeftPart(UriPartial.Path), result[i].Url.GetLeftPart(UriPartial.Path));
                Assert.AreEqual(data[i].Value, result[i].Value);
            }
        }

        [DataTestMethod]
        [TestCategory("GetPages")]
        [DataRow("asdfasedfqewqfeqwfq", DisplayName = "Rubbish url")]
        [DataRow("", DisplayName = "Empty url")]
        [DataRow(null, DisplayName = "Null url")]
        public void GetPages_ByWrongUrlString_ShouldThrow_UriFormatException(string url)
        {
            // arrange
            ReadMangaParser parser = new ReadMangaParser();
            Action action = new Action(() => parser.GetPages(url));

            // act
            // assert
            Assert.ThrowsException<UriFormatException>(action);
        }

        [DataTestMethod]
        [TestCategory("GetPages")]
        [DataRow("asdfasedfqewqfeqwfq", DisplayName = "Rubbish url")]
        [DataRow("", DisplayName = "Empty url")]
        [DataRow(null, DisplayName = "Null url")]
        public void GetPages_ByWrongChapterObjectUrl_ShouldThrow_ArgumentNullException(string url)
        {
            // arrange
            IChapter chapter = new Chapter(null, url, DateTime.Now);

            ReadMangaParser parser = new ReadMangaParser();
            Action action = new Action(() => parser.GetPages(chapter));

            // act
            // assert
            Assert.ThrowsException<ArgumentNullException>(action);
        }

        [DataTestMethod]
        [TestCategory("GetPages")]
        [DataRow("http://example.com/", DisplayName = "Wrong host url")]
        public void GetPages_ByWrongUrlStringHost_ShouldThrow_BaseHostNotMatchException(string url)
        {
            // arrange
            ReadMangaParser parser = new ReadMangaParser();
            Action action = new Action(() => parser.GetPages(url));

            // act
            // assert
            Assert.ThrowsException<BaseHostNotMatchException>(action);
        }

        [DataTestMethod]
        [TestCategory("GetPages")]
        [DataRow("http://example.com/", DisplayName = "Wrong host url")]
        public void GetPages_ByWrongUriHost_ShouldThrow_BaseHostNotMatchException(string url)
        {
            // arrange
            Uri uri = new Uri(url);

            ReadMangaParser parser = new ReadMangaParser();
            Action action = new Action(() => parser.GetPages(uri));

            // act
            // assert
            Assert.ThrowsException<BaseHostNotMatchException>(action);
        }

        [DataTestMethod]
        [TestCategory("GetPages")]
        [DataRow("http://example.com/", DisplayName = "Wrong host url")]
        public void GetPages_ByWrongChapterUrltHost_ShouldThrow_BaseHostNotMatchException(string url)
        {
            // arrange
            IChapter manga = new Chapter(null, url, DateTime.Now);

            ReadMangaParser parser = new ReadMangaParser();
            Action action = new Action(() => parser.GetPages(manga));

            // act
            // assert
            Assert.ThrowsException<BaseHostNotMatchException>(action);
        }

        #endregion Sync

        #region Async

        [DataTestMethod]
        [TestCategory("GetPages")]
        [TestCategory("Async")]
        [DataRow("https://readmanga.live/haven_t_you_heard__i_m_sakamoto/vol1/1", DisplayName = "'Haven't You Heard? I'm Sakamoto' ch1 pages")]
        [DataRow("https://readmanga.live/haven_t_you_heard__i_m_sakamoto/vol3/16", DisplayName = "'Haven't You Heard? I'm Sakamoto' ch16 pages")]
        [DataRow("https://readmanga.live/legenda_o_serebrianom_klyke_akame/vol1/1", DisplayName = "'Legend of Silver Fang Akame' ch1 pages")]
        [DataRow("https://readmanga.live/legenda_o_serebrianom_klyke_akame/vol1/7", DisplayName = "'Legend of Silver Fang Akame' ch7 pages")]
        [DataRow("https://readmanga.live/castle_town_dandelion/vol1/1?mtr=1", DisplayName = "'Castle Town Dandelion' ch1 pages")]
        [DataRow("https://readmanga.live/castle_town_dandelion/vol1/3?mtr=1", DisplayName = "'Castle Town Dandelion' ch3 pages")]
        public async Task GetPagesAsync_ByUrlString_ShouldReturn_CorrectData(string url)
        {
            // arrange
            ReadMangaParser parser = new ReadMangaParser();
            List<IDataBase> data = new List<IDataBase>();

            string dataPath = GetDataPath(parserPath, "GetPages", Uri.EscapeDataString(Path.GetRelativePath(parser.BaseUrl.OriginalString, url)));

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
                Assert.AreEqual(data[i].Url.GetLeftPart(UriPartial.Path), result[i].Url.GetLeftPart(UriPartial.Path));
                Assert.AreEqual(data[i].Value, result[i].Value);
            }
        }

        [DataTestMethod]
        [TestCategory("GetPages")]
        [TestCategory("Async")]
        [DataRow("https://readmanga.live/haven_t_you_heard__i_m_sakamoto/vol1/1", DisplayName = "'Haven't You Heard? I'm Sakamoto' ch1 pages")]
        [DataRow("https://readmanga.live/haven_t_you_heard__i_m_sakamoto/vol3/16", DisplayName = "'Haven't You Heard? I'm Sakamoto' ch16 pages")]
        [DataRow("https://readmanga.live/legenda_o_serebrianom_klyke_akame/vol1/1", DisplayName = "'Legend of Silver Fang Akame' ch1 pages")]
        [DataRow("https://readmanga.live/legenda_o_serebrianom_klyke_akame/vol1/7", DisplayName = "'Legend of Silver Fang Akame' ch7 pages")]
        [DataRow("https://readmanga.live/castle_town_dandelion/vol1/1?mtr=1", DisplayName = "'Castle Town Dandelion' ch1 pages")]
        [DataRow("https://readmanga.live/castle_town_dandelion/vol1/3?mtr=1", DisplayName = "'Castle Town Dandelion' ch3 pages")]
        public async Task GetPagesAsync_ByUri_ShouldReturn_CorrectData(string url)
        {
            // arrange
            Uri uri = new Uri(url);

            ReadMangaParser parser = new ReadMangaParser();
            List<IDataBase> data = new List<IDataBase>();

            string dataPath = GetDataPath(parserPath, "GetPages", Uri.EscapeDataString(Path.GetRelativePath(parser.BaseUrl.OriginalString, url)));

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
                Assert.AreEqual(data[i].Url.GetLeftPart(UriPartial.Path), result[i].Url.GetLeftPart(UriPartial.Path));
                Assert.AreEqual(data[i].Value, result[i].Value);
            }
        }

        [DataTestMethod]
        [TestCategory("GetPages")]
        [TestCategory("Async")]
        [DataRow("https://readmanga.live/haven_t_you_heard__i_m_sakamoto/vol1/1", DisplayName = "'Haven't You Heard? I'm Sakamoto' ch1 pages")]
        [DataRow("https://readmanga.live/haven_t_you_heard__i_m_sakamoto/vol3/16", DisplayName = "'Haven't You Heard? I'm Sakamoto' ch16 pages")]
        [DataRow("https://readmanga.live/legenda_o_serebrianom_klyke_akame/vol1/1", DisplayName = "'Legend of Silver Fang Akame' ch1 pages")]
        [DataRow("https://readmanga.live/legenda_o_serebrianom_klyke_akame/vol1/7", DisplayName = "'Legend of Silver Fang Akame' ch7 pages")]
        [DataRow("https://readmanga.live/castle_town_dandelion/vol1/1?mtr=1", DisplayName = "'Castle Town Dandelion' ch1 pages")]
        [DataRow("https://readmanga.live/castle_town_dandelion/vol1/3?mtr=1", DisplayName = "'Castle Town Dandelion' ch3 pages")]
        public async Task GetPagesAsync_ByChapter_ShouldReturn_CorrectData(string url)
        {
            // arrange
            IChapter chapter = new Chapter(null, url, DateTime.Now);

            ReadMangaParser parser = new ReadMangaParser();
            List<IDataBase> data = new List<IDataBase>();

            string dataPath = GetDataPath(parserPath, "GetPages", Uri.EscapeDataString(Path.GetRelativePath(parser.BaseUrl.OriginalString, url)));

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
                Assert.AreEqual(data[i].Url.GetLeftPart(UriPartial.Path), result[i].Url.GetLeftPart(UriPartial.Path));
                Assert.AreEqual(data[i].Value, result[i].Value);
            }
        }

        [DataTestMethod]
        [TestCategory("GetPages")]
        [TestCategory("Async")]
        [DataRow("asdfasedfqewqfeqwfq", DisplayName = "Rubbish url")]
        [DataRow("", DisplayName = "Empty url")]
        [DataRow(null, DisplayName = "Null url")]
        public async Task GetPagesAsync_ByWrongUrlString_ShouldThrow_UriFormatException(string url)
        {
            // arrange
            ReadMangaParser parser = new ReadMangaParser();
            Task<IEnumerable<IDataBase>> task()
            {
                return parser.GetPagesAsync(url);
            }

            // act
            // assert
            await Assert.ThrowsExceptionAsync<UriFormatException>(task);
        }

        [DataTestMethod]
        [TestCategory("GetPages")]
        [TestCategory("Async")]
        [DataRow("asdfasedfqewqfeqwfq", DisplayName = "Rubbish url")]
        [DataRow("", DisplayName = "Empty url")]
        [DataRow(null, DisplayName = "Null url")]
        public async Task GetPagesAsync_ByWrongChapterObjectUrl_ShouldThrow_ArgumentNullException(string url)
        {
            // arrange
            IChapter chapter = new Chapter(null, url, DateTime.Now);

            ReadMangaParser parser = new ReadMangaParser();
            Task<IEnumerable<IDataBase>> task()
            {
                return parser.GetPagesAsync(chapter);
            }
            // act
            // assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(task);
        }

        [DataTestMethod]
        [TestCategory("GetPages")]
        [TestCategory("Async")]
        [DataRow("http://example.com/", DisplayName = "Wrong host url")]
        public async Task GetPagesAsync_ByWrongUrlStringHost_ShouldThrow_BaseHostNotMatchException(string url)
        {
            // arrange
            ReadMangaParser parser = new ReadMangaParser();
            Task<IEnumerable<IDataBase>> task()
            {
                return parser.GetPagesAsync(url);
            }
            // act
            // assert
            await Assert.ThrowsExceptionAsync<BaseHostNotMatchException>(task);
        }

        [DataTestMethod]
        [TestCategory("GetPages")]
        [TestCategory("Async")]
        [DataRow("http://example.com/", DisplayName = "Wrong host url")]
        public async Task GetPagesAsync_ByWrongUriHost_ShouldThrow_BaseHostNotMatchException(string url)
        {
            // arrange
            Uri uri = new Uri(url);

            ReadMangaParser parser = new ReadMangaParser();
            Task<IEnumerable<IDataBase>> task()
            {
                return parser.GetPagesAsync(uri);
            }

            // act
            // assert
            await Assert.ThrowsExceptionAsync<BaseHostNotMatchException>(task);
        }

        [DataTestMethod]
        [TestCategory("GetPages")]
        [TestCategory("Async")]
        [DataRow("http://example.com/", DisplayName = "Wrong host url")]
        public async Task GetPagesAsync_ByWrongChapterUrltHost_ShouldThrow_BaseHostNotMatchException(string url)
        {
            // arrange
            IChapter chapter = new Chapter(null, url, DateTime.Now);

            ReadMangaParser parser = new ReadMangaParser();
            Task<IEnumerable<IDataBase>> task()
            {
                return parser.GetPagesAsync(chapter);
            }

            // act
            // assert
            await Assert.ThrowsExceptionAsync<BaseHostNotMatchException>(task);
        }

        #endregion Async

        #endregion GetPages
    }
}