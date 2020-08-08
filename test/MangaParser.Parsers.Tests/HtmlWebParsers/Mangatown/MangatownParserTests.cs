using MangaParser.Core.Interfaces;
using MangaParser.Core.Models;
using MangaParser.Parsers.HtmlWebParsers.Mangatown;
using MangaParser.Parsers.Tests.Attributes;
using MangaParser.Parsers.Tests.Converters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MangaParser.Parsers.Tests.HtmlWebParsers.Mangatown
{
    [TestClass]
    [TestClassCategory("Parser", "Mangatown")]
    public class MangatownParserTests : CoreParserTests
    {
        #region Properties

        protected override string ParserPath => "Mangatown";
        protected override IParser GetNewParser => new MangatownParser();

        #endregion Properties

        #region Search

        #region Sync

        [DataTestMethod]
        [TestMethodCategory("Search")]
        [TestMethodDelay()]
        [DataRow("sakamoto", DisplayName = "'sakamoto' query")]
        [DataRow("akame", DisplayName = "'akame' query")]
        [DataRow("Joukamachi no Dandelion", DisplayName = "Multiple word query")]
        [DataRow("asdfasedfqewqfeqwfq", DisplayName = "Rubbish query")]
        public virtual void SearchManga_WithQuery_ShouldReturn_CorrectData(string query)
        {
            // arrange
            IParser parser = GetNewParser;
            List<IMangaObject> data = new List<IMangaObject>();

            string dataPath = GetDataPath(ParserPath, "Search", String.IsNullOrEmpty(query) ? "empty" : query);

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

                var dataCovers = dataObj.Covers.ToList();
                var resultCovers = resultObj.Covers.ToList();

                for (int i = 0; i < resultCovers.Count; i++)
                {
                    Assert.AreEqual(dataCovers[i].Large?.Url?.GetLeftPart(UriPartial.Path), resultCovers[i].Large?.Url?.GetLeftPart(UriPartial.Path));
                    Assert.AreEqual(dataCovers[i].Medium?.Url?.GetLeftPart(UriPartial.Path), resultCovers[i].Medium?.Url?.GetLeftPart(UriPartial.Path));
                    Assert.AreEqual(dataCovers[i].Small?.Url?.GetLeftPart(UriPartial.Path), resultCovers[i].Small?.Url?.GetLeftPart(UriPartial.Path));
                }

                CollectionAssert.AreEquivalent(dataObj.Authors.ToList(), resultObj.Authors.ToList());
                CollectionAssert.AreEquivalent(dataObj.Genres.ToList(), resultObj.Genres.ToList());
                Assert.AreEqual(dataObj.Description, resultObj.Description);
                CollectionAssert.AreEquivalent(dataObj.Illustrators.ToList(), resultObj.Illustrators.ToList());
                CollectionAssert.AreEquivalent(dataObj.Writers.ToList(), resultObj.Writers.ToList());
                CollectionAssert.AreEquivalent(dataObj.Magazines.ToList(), resultObj.Magazines.ToList());
                CollectionAssert.AreEquivalent(dataObj.Publishers.ToList(), resultObj.Publishers.ToList());
                Assert.AreEqual(dataObj.ReleaseDate, resultObj.ReleaseDate);
                Assert.AreEqual(dataObj.Volumes, resultObj.Volumes);
            }
        }

        [DataTestMethod]
        [TestMethodCategory("Search")]
        [TestMethodDelay()]
        [DataRow("", DisplayName = "Empty query")]
        [DataRow(null, DisplayName = "Null query")]
        public virtual void SearchManga_WithEmptyQuery_ShouldReturn_CorrectData(string query)
        {
            // arrange
            IParser parser = GetNewParser;

            // act
            var result = parser.SearchManga(query).ToList();

            // assert
            Assert.IsTrue(result.Count > 0);
            CollectionAssert.AllItemsAreUnique(result);

            foreach (IMangaObject resultObj in result)
            {
                CollectionAssert.AllItemsAreUnique(resultObj.Covers.ToList());
                CollectionAssert.AllItemsAreUnique(resultObj.Authors.ToList());
                CollectionAssert.AllItemsAreUnique(resultObj.Genres.ToList());
                Assert.AreEqual(null, resultObj.Description);
                CollectionAssert.AllItemsAreUnique(resultObj.Illustrators.ToList());
                CollectionAssert.AllItemsAreUnique(resultObj.Writers.ToList());
                CollectionAssert.AllItemsAreUnique(resultObj.Magazines.ToList());
                CollectionAssert.AllItemsAreUnique(resultObj.Publishers.ToList());
                Assert.AreEqual(null, resultObj.ReleaseDate);
                Assert.AreEqual(null, resultObj.Volumes);
            }
        }

        #endregion Sync

        #region Async

        [DataTestMethod]
        [TestMethodCategory("Search", "Async")]
        [TestMethodDelay()]
        [DataRow("sakamoto", DisplayName = "'sakamoto' query")]
        [DataRow("akame", DisplayName = "'akame' query")]
        [DataRow("Joukamachi no Dandelion", DisplayName = "Multiple word query")]
        [DataRow("asdfasedfqewqfeqwfq", DisplayName = "Rubbish query")]
        public virtual async Task SearchMangaAsync_WithQuery_ShouldReturn_CorrectData(string query)
        {
            // arrange
            IParser parser = GetNewParser;
            List<IMangaObject> data = new List<IMangaObject>();

            string dataPath = GetDataPath(ParserPath, "Search", String.IsNullOrEmpty(query) ? "empty" : query);

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

                var dataCovers = dataObj.Covers.ToList();
                var resultCovers = resultObj.Covers.ToList();

                for (int i = 0; i < resultCovers.Count; i++)
                {
                    Assert.AreEqual(dataCovers[i].Large?.Url?.GetLeftPart(UriPartial.Path), resultCovers[i].Large?.Url?.GetLeftPart(UriPartial.Path));
                    Assert.AreEqual(dataCovers[i].Medium?.Url?.GetLeftPart(UriPartial.Path), resultCovers[i].Medium?.Url?.GetLeftPart(UriPartial.Path));
                    Assert.AreEqual(dataCovers[i].Small?.Url?.GetLeftPart(UriPartial.Path), resultCovers[i].Small?.Url?.GetLeftPart(UriPartial.Path));
                }

                CollectionAssert.AreEquivalent(dataObj.Authors.ToList(), resultObj.Authors.ToList());
                CollectionAssert.AreEquivalent(dataObj.Genres.ToList(), resultObj.Genres.ToList());
                Assert.AreEqual(dataObj.Description, resultObj.Description);
                CollectionAssert.AreEquivalent(dataObj.Illustrators.ToList(), resultObj.Illustrators.ToList());
                CollectionAssert.AreEquivalent(dataObj.Writers.ToList(), resultObj.Writers.ToList());
                CollectionAssert.AreEquivalent(dataObj.Magazines.ToList(), resultObj.Magazines.ToList());
                CollectionAssert.AreEquivalent(dataObj.Publishers.ToList(), resultObj.Publishers.ToList());
                Assert.AreEqual(dataObj.ReleaseDate, resultObj.ReleaseDate);
                Assert.AreEqual(dataObj.Volumes, resultObj.Volumes);
            }
        }

        [DataTestMethod]
        [TestMethodCategory("Search", "Async")]
        [TestMethodDelay()]
        [DataRow("", DisplayName = "Empty query")]
        [DataRow(null, DisplayName = "Null query")]
        public virtual async Task SearchMangaAsync_WithEmptyQuery_ShouldReturn_CorrectData(string query)
        {
            // arrange
            IParser parser = GetNewParser;

            // act
            var result = (await parser.SearchMangaAsync(query)).ToList();

            // assert
            Assert.IsTrue(result.Count > 0);
            CollectionAssert.AllItemsAreUnique(result);

            foreach (IMangaObject resultObj in result)
            {
                CollectionAssert.AllItemsAreUnique(resultObj.Covers.ToList());
                CollectionAssert.AllItemsAreUnique(resultObj.Authors.ToList());
                CollectionAssert.AllItemsAreUnique(resultObj.Genres.ToList());
                Assert.AreEqual(null, resultObj.Description);
                CollectionAssert.AllItemsAreUnique(resultObj.Illustrators.ToList());
                CollectionAssert.AllItemsAreUnique(resultObj.Writers.ToList());
                CollectionAssert.AllItemsAreUnique(resultObj.Magazines.ToList());
                CollectionAssert.AllItemsAreUnique(resultObj.Publishers.ToList());
                Assert.AreEqual(null, resultObj.ReleaseDate);
                Assert.AreEqual(null, resultObj.Volumes);
            }
        }

        #endregion Async

        #endregion Search

        #region GetManga

        #region Sync

        [DataTestMethod]
        [TestMethodCategory("GetManga")]
        [TestMethodDelay()]
        [DataRow("https://www.mangatown.com/manga/sakamoto_desu_ga", DisplayName = "'Sakamoto desu ga?' url")]
        [DataRow("https://www.mangatown.com/manga/akame_ga_kiru", DisplayName = "'Akame ga Kill!' url")]
        [DataRow("https://www.mangatown.com/manga/joukamachi_no_dandelion", DisplayName = "'Joukamachi no Dandelion' url")]
        [DataRow("https://www.mangatown.com/manga/onepunch_man", DisplayName = "'Onepunch-Man' url")]
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

            var dataCovers = data.Covers.ToList();
            var resultCovers = result.Covers.ToList();

            for (int i = 0; i < resultCovers.Count; i++)
            {
                Assert.AreEqual(dataCovers[i].Large?.Url?.GetLeftPart(UriPartial.Path), resultCovers[i].Large?.Url?.GetLeftPart(UriPartial.Path));
                Assert.AreEqual(dataCovers[i].Medium?.Url?.GetLeftPart(UriPartial.Path), resultCovers[i].Medium?.Url?.GetLeftPart(UriPartial.Path));
                Assert.AreEqual(dataCovers[i].Small?.Url?.GetLeftPart(UriPartial.Path), resultCovers[i].Small?.Url?.GetLeftPart(UriPartial.Path));
            }

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
        [DataRow("https://www.mangatown.com/manga/sakamoto_desu_ga", DisplayName = "'Sakamoto desu ga?' url")]
        [DataRow("https://www.mangatown.com/manga/akame_ga_kiru", DisplayName = "'Akame ga Kill!' url")]
        [DataRow("https://www.mangatown.com/manga/joukamachi_no_dandelion", DisplayName = "'Joukamachi no Dandelion' url")]
        [DataRow("https://www.mangatown.com/manga/onepunch_man", DisplayName = "'Onepunch-Man' url")]
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

            var dataCovers = data.Covers.ToList();
            var resultCovers = result.Covers.ToList();

            for (int i = 0; i < resultCovers.Count; i++)
            {
                Assert.AreEqual(dataCovers[i].Large?.Url?.GetLeftPart(UriPartial.Path), resultCovers[i].Large?.Url?.GetLeftPart(UriPartial.Path));
                Assert.AreEqual(dataCovers[i].Medium?.Url?.GetLeftPart(UriPartial.Path), resultCovers[i].Medium?.Url?.GetLeftPart(UriPartial.Path));
                Assert.AreEqual(dataCovers[i].Small?.Url?.GetLeftPart(UriPartial.Path), resultCovers[i].Small?.Url?.GetLeftPart(UriPartial.Path));
            }

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
        [DataRow("https://www.mangatown.com/manga/sakamoto_desu_ga", DisplayName = "'Sakamoto desu ga?' url")]
        [DataRow("https://www.mangatown.com/manga/akame_ga_kiru", DisplayName = "'Akame ga Kill!' url")]
        [DataRow("https://www.mangatown.com/manga/joukamachi_no_dandelion", DisplayName = "'Joukamachi no Dandelion' url")]
        [DataRow("https://www.mangatown.com/manga/onepunch_man", DisplayName = "'Onepunch-Man' url")]
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

            var dataCovers = data.Covers.ToList();
            var resultCovers = result.Covers.ToList();

            for (int i = 0; i < resultCovers.Count; i++)
            {
                Assert.AreEqual(dataCovers[i].Large?.Url?.GetLeftPart(UriPartial.Path), resultCovers[i].Large?.Url?.GetLeftPart(UriPartial.Path));
                Assert.AreEqual(dataCovers[i].Medium?.Url?.GetLeftPart(UriPartial.Path), resultCovers[i].Medium?.Url?.GetLeftPart(UriPartial.Path));
                Assert.AreEqual(dataCovers[i].Small?.Url?.GetLeftPart(UriPartial.Path), resultCovers[i].Small?.Url?.GetLeftPart(UriPartial.Path));
            }

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
        [DataRow("https://www.mangatown.com/manga/sakamoto_desu_ga", DisplayName = "'Sakamoto desu ga?' url")]
        [DataRow("https://www.mangatown.com/manga/akame_ga_kiru", DisplayName = "'Akame ga Kill!' url")]
        [DataRow("https://www.mangatown.com/manga/joukamachi_no_dandelion", DisplayName = "'Joukamachi no Dandelion' url")]
        [DataRow("https://www.mangatown.com/manga/onepunch_man", DisplayName = "'Onepunch-Man' url")]
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

            var dataCovers = data.Covers.ToList();
            var resultCovers = result.Covers.ToList();

            for (int i = 0; i < resultCovers.Count; i++)
            {
                Assert.AreEqual(dataCovers[i].Large?.Url?.GetLeftPart(UriPartial.Path), resultCovers[i].Large?.Url?.GetLeftPart(UriPartial.Path));
                Assert.AreEqual(dataCovers[i].Medium?.Url?.GetLeftPart(UriPartial.Path), resultCovers[i].Medium?.Url?.GetLeftPart(UriPartial.Path));
                Assert.AreEqual(dataCovers[i].Small?.Url?.GetLeftPart(UriPartial.Path), resultCovers[i].Small?.Url?.GetLeftPart(UriPartial.Path));
            }

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
        [DataRow("https://www.mangatown.com/manga/sakamoto_desu_ga", DisplayName = "'Sakamoto desu ga?' url")]
        [DataRow("https://www.mangatown.com/manga/akame_ga_kiru", DisplayName = "'Akame ga Kill!' url")]
        [DataRow("https://www.mangatown.com/manga/joukamachi_no_dandelion", DisplayName = "'Joukamachi no Dandelion' url")]
        [DataRow("https://www.mangatown.com/manga/onepunch_man", DisplayName = "'Onepunch-Man' url")]
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

            var dataCovers = data.Covers.ToList();
            var resultCovers = result.Covers.ToList();

            for (int i = 0; i < resultCovers.Count; i++)
            {
                Assert.AreEqual(dataCovers[i].Large?.Url?.GetLeftPart(UriPartial.Path), resultCovers[i].Large?.Url?.GetLeftPart(UriPartial.Path));
                Assert.AreEqual(dataCovers[i].Medium?.Url?.GetLeftPart(UriPartial.Path), resultCovers[i].Medium?.Url?.GetLeftPart(UriPartial.Path));
                Assert.AreEqual(dataCovers[i].Small?.Url?.GetLeftPart(UriPartial.Path), resultCovers[i].Small?.Url?.GetLeftPart(UriPartial.Path));
            }

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
        [DataRow("https://www.mangatown.com/manga/sakamoto_desu_ga", DisplayName = "'Sakamoto desu ga?' url")]
        [DataRow("https://www.mangatown.com/manga/akame_ga_kiru", DisplayName = "'Akame ga Kill!' url")]
        [DataRow("https://www.mangatown.com/manga/joukamachi_no_dandelion", DisplayName = "'Joukamachi no Dandelion' url")]
        [DataRow("https://www.mangatown.com/manga/onepunch_man", DisplayName = "'Onepunch-Man' url")]
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

            var dataCovers = data.Covers.ToList();
            var resultCovers = result.Covers.ToList();

            for (int i = 0; i < resultCovers.Count; i++)
            {
                Assert.AreEqual(dataCovers[i].Large?.Url?.GetLeftPart(UriPartial.Path), resultCovers[i].Large?.Url?.GetLeftPart(UriPartial.Path));
                Assert.AreEqual(dataCovers[i].Medium?.Url?.GetLeftPart(UriPartial.Path), resultCovers[i].Medium?.Url?.GetLeftPart(UriPartial.Path));
                Assert.AreEqual(dataCovers[i].Small?.Url?.GetLeftPart(UriPartial.Path), resultCovers[i].Small?.Url?.GetLeftPart(UriPartial.Path));
            }

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
        [DataRow("https://www.mangatown.com/manga/sakamoto_desu_ga", DisplayName = "'Sakamoto desu ga?' chapters")]
        [DataRow("https://www.mangatown.com/manga/akame_ga_kiru", DisplayName = "'Akame ga Kill!' chapters")]
        [DataRow("https://www.mangatown.com/manga/joukamachi_no_dandelion", DisplayName = "'Joukamachi no Dandelion' chapters")]
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
        [DataRow("https://www.mangatown.com/manga/sakamoto_desu_ga", DisplayName = "'Sakamoto desu ga?' chapters")]
        [DataRow("https://www.mangatown.com/manga/akame_ga_kiru", DisplayName = "'Akame ga Kill!' chapters")]
        [DataRow("https://www.mangatown.com/manga/joukamachi_no_dandelion", DisplayName = "'Joukamachi no Dandelion' chapters")]
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
        [DataRow("https://www.mangatown.com/manga/sakamoto_desu_ga", DisplayName = "'Sakamoto desu ga?' chapters")]
        [DataRow("https://www.mangatown.com/manga/akame_ga_kiru", DisplayName = "'Akame ga Kill!' chapters")]
        [DataRow("https://www.mangatown.com/manga/joukamachi_no_dandelion", DisplayName = "'Joukamachi no Dandelion' chapters")]
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
        [DataRow("https://www.mangatown.com/manga/sakamoto_desu_ga", DisplayName = "'Sakamoto desu ga?' chapters")]
        [DataRow("https://www.mangatown.com/manga/akame_ga_kiru", DisplayName = "'Akame ga Kill!' chapters")]
        [DataRow("https://www.mangatown.com/manga/joukamachi_no_dandelion", DisplayName = "'Joukamachi no Dandelion' chapters")]
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
        [DataRow("https://www.mangatown.com/manga/sakamoto_desu_ga", DisplayName = "'Sakamoto desu ga?' chapters")]
        [DataRow("https://www.mangatown.com/manga/akame_ga_kiru", DisplayName = "'Akame ga Kill!' chapters")]
        [DataRow("https://www.mangatown.com/manga/joukamachi_no_dandelion", DisplayName = "'Joukamachi no Dandelion' chapters")]
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
        [DataRow("https://www.mangatown.com/manga/sakamoto_desu_ga", DisplayName = "'Sakamoto desu ga?' chapters")]
        [DataRow("https://www.mangatown.com/manga/akame_ga_kiru", DisplayName = "'Akame ga Kill!' chapters")]
        [DataRow("https://www.mangatown.com/manga/joukamachi_no_dandelion", DisplayName = "'Joukamachi no Dandelion' chapters")]
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
        [DataRow("https://www.mangatown.com/manga/sakamoto_desu_ga/c001", DisplayName = "'Sakamoto desu ga?' ch1 pages")]
        [DataRow("https://www.mangatown.com/manga/sakamoto_desu_ga/c016", DisplayName = "'Sakamoto desu ga?' ch16 pages")]
        [DataRow("https://www.mangatown.com/manga/akame_ga_kiru/c001", DisplayName = "'Akame ga Kill!' ch1 pages")]
        [DataRow("https://www.mangatown.com/manga/akame_ga_kiru/c007", DisplayName = "'Akame ga Kill!' ch7 pages")]
        [DataRow("https://www.mangatown.com/manga/joukamachi_no_dandelion/c001", DisplayName = "'Joukamachi no Dandelion' ch1 pages")]
        [DataRow("https://www.mangatown.com/manga/joukamachi_no_dandelion/c044", DisplayName = "'Joukamachi no Dandelion' ch44 pages")]
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
                Assert.AreEqual(data[i].Url.GetLeftPart(UriPartial.Path), result[i].Url.GetLeftPart(UriPartial.Path));
                Assert.AreEqual(data[i].Value, result[i].Value);
            }
        }

        [DataTestMethod]
        [TestMethodCategory("GetPages")]
        [TestMethodDelay()]
        [DataRow("https://www.mangatown.com/manga/sakamoto_desu_ga/c001", DisplayName = "'Sakamoto desu ga?' ch1 pages")]
        [DataRow("https://www.mangatown.com/manga/sakamoto_desu_ga/c016", DisplayName = "'Sakamoto desu ga?' ch16 pages")]
        [DataRow("https://www.mangatown.com/manga/akame_ga_kiru/c001", DisplayName = "'Akame ga Kill!' ch1 pages")]
        [DataRow("https://www.mangatown.com/manga/akame_ga_kiru/c007", DisplayName = "'Akame ga Kill!' ch7 pages")]
        [DataRow("https://www.mangatown.com/manga/joukamachi_no_dandelion/c001", DisplayName = "'Joukamachi no Dandelion' ch1 pages")]
        [DataRow("https://www.mangatown.com/manga/joukamachi_no_dandelion/c044", DisplayName = "'Joukamachi no Dandelion' ch44 pages")]
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
                Assert.AreEqual(data[i].Url.GetLeftPart(UriPartial.Path), result[i].Url.GetLeftPart(UriPartial.Path));
                Assert.AreEqual(data[i].Value, result[i].Value);
            }
        }

        [DataTestMethod]
        [TestMethodCategory("GetPages")]
        [TestMethodDelay()]
        [DataRow("https://www.mangatown.com/manga/sakamoto_desu_ga/c001", DisplayName = "'Sakamoto desu ga?' ch1 pages")]
        [DataRow("https://www.mangatown.com/manga/sakamoto_desu_ga/c016", DisplayName = "'Sakamoto desu ga?' ch16 pages")]
        [DataRow("https://www.mangatown.com/manga/akame_ga_kiru/c001", DisplayName = "'Akame ga Kill!' ch1 pages")]
        [DataRow("https://www.mangatown.com/manga/akame_ga_kiru/c007", DisplayName = "'Akame ga Kill!' ch7 pages")]
        [DataRow("https://www.mangatown.com/manga/joukamachi_no_dandelion/c001", DisplayName = "'Joukamachi no Dandelion' ch1 pages")]
        [DataRow("https://www.mangatown.com/manga/joukamachi_no_dandelion/c044", DisplayName = "'Joukamachi no Dandelion' ch44 pages")]
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
                Assert.AreEqual(data[i].Url.GetLeftPart(UriPartial.Path), result[i].Url.GetLeftPart(UriPartial.Path));
                Assert.AreEqual(data[i].Value, result[i].Value);
            }
        }

        #endregion Sync

        #region Async

        [DataTestMethod]
        [TestMethodCategory("GetPages", "Async")]
        [TestMethodDelay()]
        [DataRow("https://www.mangatown.com/manga/sakamoto_desu_ga/c001", DisplayName = "'Sakamoto desu ga?' ch1 pages")]
        [DataRow("https://www.mangatown.com/manga/sakamoto_desu_ga/c016", DisplayName = "'Sakamoto desu ga?' ch16 pages")]
        [DataRow("https://www.mangatown.com/manga/akame_ga_kiru/c001", DisplayName = "'Akame ga Kill!' ch1 pages")]
        [DataRow("https://www.mangatown.com/manga/akame_ga_kiru/c007", DisplayName = "'Akame ga Kill!' ch7 pages")]
        [DataRow("https://www.mangatown.com/manga/joukamachi_no_dandelion/c001", DisplayName = "'Joukamachi no Dandelion' ch1 pages")]
        [DataRow("https://www.mangatown.com/manga/joukamachi_no_dandelion/c044", DisplayName = "'Joukamachi no Dandelion' ch44 pages")]
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
                Assert.AreEqual(data[i].Url.GetLeftPart(UriPartial.Path), result[i].Url.GetLeftPart(UriPartial.Path));
                Assert.AreEqual(data[i].Value, result[i].Value);
            }
        }

        [DataTestMethod]
        [TestMethodCategory("GetPages", "Async")]
        [TestMethodDelay()]
        [DataRow("https://www.mangatown.com/manga/sakamoto_desu_ga/c001", DisplayName = "'Sakamoto desu ga?' ch1 pages")]
        [DataRow("https://www.mangatown.com/manga/sakamoto_desu_ga/c016", DisplayName = "'Sakamoto desu ga?' ch16 pages")]
        [DataRow("https://www.mangatown.com/manga/akame_ga_kiru/c001", DisplayName = "'Akame ga Kill!' ch1 pages")]
        [DataRow("https://www.mangatown.com/manga/akame_ga_kiru/c007", DisplayName = "'Akame ga Kill!' ch7 pages")]
        [DataRow("https://www.mangatown.com/manga/joukamachi_no_dandelion/c001", DisplayName = "'Joukamachi no Dandelion' ch1 pages")]
        [DataRow("https://www.mangatown.com/manga/joukamachi_no_dandelion/c044", DisplayName = "'Joukamachi no Dandelion' ch44 pages")]
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
                Assert.AreEqual(data[i].Url.GetLeftPart(UriPartial.Path), result[i].Url.GetLeftPart(UriPartial.Path));
                Assert.AreEqual(data[i].Value, result[i].Value);
            }
        }

        [DataTestMethod]
        [TestMethodCategory("GetPages", "Async")]
        [TestMethodDelay()]
        [DataRow("https://www.mangatown.com/manga/sakamoto_desu_ga/c001", DisplayName = "'Sakamoto desu ga?' ch1 pages")]
        [DataRow("https://www.mangatown.com/manga/sakamoto_desu_ga/c016", DisplayName = "'Sakamoto desu ga?' ch16 pages")]
        [DataRow("https://www.mangatown.com/manga/akame_ga_kiru/c001", DisplayName = "'Akame ga Kill!' ch1 pages")]
        [DataRow("https://www.mangatown.com/manga/akame_ga_kiru/c007", DisplayName = "'Akame ga Kill!' ch7 pages")]
        [DataRow("https://www.mangatown.com/manga/joukamachi_no_dandelion/c001", DisplayName = "'Joukamachi no Dandelion' ch1 pages")]
        [DataRow("https://www.mangatown.com/manga/joukamachi_no_dandelion/c044", DisplayName = "'Joukamachi no Dandelion' ch44 pages")]
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
                Assert.AreEqual(data[i].Url.GetLeftPart(UriPartial.Path), result[i].Url.GetLeftPart(UriPartial.Path));
                Assert.AreEqual(data[i].Value, result[i].Value);
            }
        }

        #endregion Async

        #endregion GetPages
    }
}