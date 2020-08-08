using MangaParser.Core.Interfaces;
using MangaParser.Core.Models;
using MangaParser.Parsers.HtmlWebParsers.Mangareader;
using MangaParser.Parsers.Tests.Attributes;
using MangaParser.Parsers.Tests.Converters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MangaParser.Parsers.Tests.HtmlWebParsers.Mangareader
{
    [TestClass]
    [TestClassCategory("Parser", "Mangareader")]
    public class MangareaderParserTests : CoreParserTests
    {
        #region Properties

        protected override string ParserPath => "Mangareader";
        protected override IParser GetNewParser => new MangareaderParser();

        #endregion Properties

        #region Search

        #region Sync

        [DataTestMethod]
        [TestMethodCategory("Search")]
        [TestMethodDelay(5000)]
        [DataRow("sakamotodesu", DisplayName = "'sakamotodesu' query")]
        [DataRow("akame", DisplayName = "'akame' query")]
        [DataRow("onepunch man", DisplayName = "Multiple word query")]
        [DataRow("asdfasedfqewqfeqwfq", DisplayName = "Rubbish query")]
        [DataRow("", DisplayName = "Empty query")]
        [DataRow(null, DisplayName = "Null query")]
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

                CollectionAssert.AreEquivalent(dataObj.Covers.ToList(), resultObj.Covers.ToList());
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

        #endregion Sync

        #region Async

        [DataTestMethod]
        [TestMethodCategory("Search", "Async")]
        [TestMethodDelay(5000)]
        [DataRow("sakamotodesu", DisplayName = "'sakamotodesu' query")]
        [DataRow("akame", DisplayName = "'akame' query")]
        [DataRow("onepunch man", DisplayName = "Multiple word query")]
        [DataRow("asdfasedfqewqfeqwfq", DisplayName = "Rubbish query")]
        [DataRow("", DisplayName = "Empty query")]
        [DataRow(null, DisplayName = "Null query")]
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

                CollectionAssert.AreEquivalent(dataObj.Covers.ToList(), resultObj.Covers.ToList());
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

        #endregion Async

        #endregion Search

        #region GetManga

        #region Sync

        [DataTestMethod]
        [TestMethodCategory("GetManga")]
        [TestMethodDelay(5000)]
        [DataRow("http://www.mangareader.net/sakamoto-desu-ga", DisplayName = "'Sakamoto desu ga?' url")]
        [DataRow("http://www.mangareader.net/akame-ga-kiru", DisplayName = "'Akame ga Kill!' url")]
        [DataRow("http://www.mangareader.net/nisekoi", DisplayName = "'Nisekoi' url")]
        [DataRow("http://www.mangareader.net/onepunch-man", DisplayName = "'Onepunch-Man' url")]
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
        [TestMethodDelay(5000)]
        [DataRow("http://www.mangareader.net/sakamoto-desu-ga", DisplayName = "'Sakamoto desu ga?' url")]
        [DataRow("http://www.mangareader.net/akame-ga-kiru", DisplayName = "'Akame ga Kill!' url")]
        [DataRow("http://www.mangareader.net/nisekoi", DisplayName = "'Nisekoi' url")]
        [DataRow("http://www.mangareader.net/onepunch-man", DisplayName = "'Onepunch-Man' url")]
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
        [TestMethodDelay(5000)]
        [DataRow("http://www.mangareader.net/sakamoto-desu-ga", DisplayName = "'Sakamoto desu ga?' url")]
        [DataRow("http://www.mangareader.net/akame-ga-kiru", DisplayName = "'Akame ga Kill!' url")]
        [DataRow("http://www.mangareader.net/nisekoi", DisplayName = "'Nisekoi' url")]
        [DataRow("http://www.mangareader.net/onepunch-man", DisplayName = "'Onepunch-Man' url")]
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
        [TestMethodDelay(5000)]
        [DataRow("http://www.mangareader.net/sakamoto-desu-ga", DisplayName = "'Sakamoto desu ga?' url")]
        [DataRow("http://www.mangareader.net/akame-ga-kiru", DisplayName = "'Akame ga Kill!' url")]
        [DataRow("http://www.mangareader.net/nisekoi", DisplayName = "'Nisekoi' url")]
        [DataRow("http://www.mangareader.net/onepunch-man", DisplayName = "'Onepunch-Man' url")]
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
        [TestMethodDelay(5000)]
        [DataRow("http://www.mangareader.net/sakamoto-desu-ga", DisplayName = "'Sakamoto desu ga?' url")]
        [DataRow("http://www.mangareader.net/akame-ga-kiru", DisplayName = "'Akame ga Kill!' url")]
        [DataRow("http://www.mangareader.net/nisekoi", DisplayName = "'Nisekoi' url")]
        [DataRow("http://www.mangareader.net/onepunch-man", DisplayName = "'Onepunch-Man' url")]
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
        [TestMethodDelay(5000)]
        [DataRow("http://www.mangareader.net/sakamoto-desu-ga", DisplayName = "'Sakamoto desu ga?' url")]
        [DataRow("http://www.mangareader.net/akame-ga-kiru", DisplayName = "'Akame ga Kill!' url")]
        [DataRow("http://www.mangareader.net/nisekoi", DisplayName = "'Nisekoi' url")]
        [DataRow("http://www.mangareader.net/onepunch-man", DisplayName = "'Onepunch-Man' url")]
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
        [TestMethodDelay(5000)]
        [DataRow("http://www.mangareader.net/sakamoto-desu-ga", DisplayName = "'Sakamoto desu ga?' chapters")]
        [DataRow("http://www.mangareader.net/akame-ga-kiru", DisplayName = "'Akame ga Kill!' chapters")]
        [DataRow("http://www.mangareader.net/nisekoi", DisplayName = "'Nisekoi' chapters")]
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
        [TestMethodDelay(5000)]
        [DataRow("http://www.mangareader.net/sakamoto-desu-ga", DisplayName = "'Sakamoto desu ga?' chapters")]
        [DataRow("http://www.mangareader.net/akame-ga-kiru", DisplayName = "'Akame ga Kill!' chapters")]
        [DataRow("http://www.mangareader.net/nisekoi", DisplayName = "'Nisekoi' chapters")]
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
        [TestMethodDelay(5000)]
        [DataRow("http://www.mangareader.net/sakamoto-desu-ga", DisplayName = "'Sakamoto desu ga?' chapters")]
        [DataRow("http://www.mangareader.net/akame-ga-kiru", DisplayName = "'Akame ga Kill!' chapters")]
        [DataRow("http://www.mangareader.net/nisekoi", DisplayName = "'Nisekoi' chapters")]
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
        [TestMethodDelay(5000)]
        [DataRow("http://www.mangareader.net/sakamoto-desu-ga", DisplayName = "'Sakamoto desu ga?' chapters")]
        [DataRow("http://www.mangareader.net/akame-ga-kiru", DisplayName = "'Akame ga Kill!' chapters")]
        [DataRow("http://www.mangareader.net/nisekoi", DisplayName = "'Nisekoi' chapters")]
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
        [TestMethodDelay(5000)]
        [DataRow("http://www.mangareader.net/sakamoto-desu-ga", DisplayName = "'Sakamoto desu ga?' chapters")]
        [DataRow("http://www.mangareader.net/akame-ga-kiru", DisplayName = "'Akame ga Kill!' chapters")]
        [DataRow("http://www.mangareader.net/nisekoi", DisplayName = "'Nisekoi' chapters")]
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
        [TestMethodDelay(5000)]
        [DataRow("http://www.mangareader.net/sakamoto-desu-ga", DisplayName = "'Sakamoto desu ga?' chapters")]
        [DataRow("http://www.mangareader.net/akame-ga-kiru", DisplayName = "'Akame ga Kill!' chapters")]
        [DataRow("http://www.mangareader.net/nisekoi", DisplayName = "'Nisekoi' chapters")]
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
        [TestMethodDelay(5000)]
        [DataRow("http://www.mangareader.net/sakamoto-desu-ga/1", DisplayName = "'Sakamoto desu ga?' ch1 pages")]
        [DataRow("http://www.mangareader.net/sakamoto-desu-ga/16", DisplayName = "'Sakamoto desu ga?' ch16 pages")]
        [DataRow("http://www.mangareader.net/akame-ga-kiru/1", DisplayName = "'Akame ga Kill!' ch1 pages")]
        [DataRow("http://www.mangareader.net/akame-ga-kiru/7", DisplayName = "'Akame ga Kill!' ch7 pages")]
        [DataRow("http://www.mangareader.net/nisekoi/1", DisplayName = "'Nisekoi' ch1 pages")]
        [DataRow("http://www.mangareader.net/nisekoi/44", DisplayName = "'Nisekoi' ch44 pages")]
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
                Assert.AreEqual(data[i].Url, result[i].Url);
                Assert.AreEqual(data[i].Value, result[i].Value);
            }
        }

        [DataTestMethod]
        [TestMethodCategory("GetPages")]
        [TestMethodDelay(5000)]
        [DataRow("http://www.mangareader.net/sakamoto-desu-ga/1", DisplayName = "'Sakamoto desu ga?' ch1 pages")]
        [DataRow("http://www.mangareader.net/sakamoto-desu-ga/16", DisplayName = "'Sakamoto desu ga?' ch16 pages")]
        [DataRow("http://www.mangareader.net/akame-ga-kiru/1", DisplayName = "'Akame ga Kill!' ch1 pages")]
        [DataRow("http://www.mangareader.net/akame-ga-kiru/7", DisplayName = "'Akame ga Kill!' ch7 pages")]
        [DataRow("http://www.mangareader.net/nisekoi/1", DisplayName = "'Nisekoi' ch1 pages")]
        [DataRow("http://www.mangareader.net/nisekoi/44", DisplayName = "'Nisekoi' ch44 pages")]
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
                Assert.AreEqual(data[i].Url, result[i].Url);
                Assert.AreEqual(data[i].Value, result[i].Value);
            }
        }

        [DataTestMethod]
        [TestMethodCategory("GetPages")]
        [TestMethodDelay(5000)]
        [DataRow("http://www.mangareader.net/sakamoto-desu-ga/1", DisplayName = "'Sakamoto desu ga?' ch1 pages")]
        [DataRow("http://www.mangareader.net/sakamoto-desu-ga/16", DisplayName = "'Sakamoto desu ga?' ch16 pages")]
        [DataRow("http://www.mangareader.net/akame-ga-kiru/1", DisplayName = "'Akame ga Kill!' ch1 pages")]
        [DataRow("http://www.mangareader.net/akame-ga-kiru/7", DisplayName = "'Akame ga Kill!' ch7 pages")]
        [DataRow("http://www.mangareader.net/nisekoi/1", DisplayName = "'Nisekoi' ch1 pages")]
        [DataRow("http://www.mangareader.net/nisekoi/44", DisplayName = "'Nisekoi' ch44 pages")]
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
                Assert.AreEqual(data[i].Url, result[i].Url);
                Assert.AreEqual(data[i].Value, result[i].Value);
            }
        }

        #endregion Sync

        #region Async

        [DataTestMethod]
        [TestMethodCategory("GetPages", "Async")]
        [TestMethodDelay(5000)]
        [DataRow("http://www.mangareader.net/sakamoto-desu-ga/1", DisplayName = "'Sakamoto desu ga?' ch1 pages")]
        [DataRow("http://www.mangareader.net/sakamoto-desu-ga/16", DisplayName = "'Sakamoto desu ga?' ch16 pages")]
        [DataRow("http://www.mangareader.net/akame-ga-kiru/1", DisplayName = "'Akame ga Kill!' ch1 pages")]
        [DataRow("http://www.mangareader.net/akame-ga-kiru/7", DisplayName = "'Akame ga Kill!' ch7 pages")]
        [DataRow("http://www.mangareader.net/nisekoi/1", DisplayName = "'Nisekoi' ch1 pages")]
        [DataRow("http://www.mangareader.net/nisekoi/44", DisplayName = "'Nisekoi' ch44 pages")]
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
                Assert.AreEqual(data[i].Url, result[i].Url);
                Assert.AreEqual(data[i].Value, result[i].Value);
            }
        }

        [DataTestMethod]
        [TestMethodCategory("GetPages", "Async")]
        [TestMethodDelay(5000)]
        [DataRow("http://www.mangareader.net/sakamoto-desu-ga/1", DisplayName = "'Sakamoto desu ga?' ch1 pages")]
        [DataRow("http://www.mangareader.net/sakamoto-desu-ga/16", DisplayName = "'Sakamoto desu ga?' ch16 pages")]
        [DataRow("http://www.mangareader.net/akame-ga-kiru/1", DisplayName = "'Akame ga Kill!' ch1 pages")]
        [DataRow("http://www.mangareader.net/akame-ga-kiru/7", DisplayName = "'Akame ga Kill!' ch7 pages")]
        [DataRow("http://www.mangareader.net/nisekoi/1", DisplayName = "'Nisekoi' ch1 pages")]
        [DataRow("http://www.mangareader.net/nisekoi/44", DisplayName = "'Nisekoi' ch44 pages")]
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
                Assert.AreEqual(data[i].Url, result[i].Url);
                Assert.AreEqual(data[i].Value, result[i].Value);
            }
        }

        [DataTestMethod]
        [TestMethodCategory("GetPages", "Async")]
        [TestMethodDelay(5000)]
        [DataRow("http://www.mangareader.net/sakamoto-desu-ga/1", DisplayName = "'Sakamoto desu ga?' ch1 pages")]
        [DataRow("http://www.mangareader.net/sakamoto-desu-ga/16", DisplayName = "'Sakamoto desu ga?' ch16 pages")]
        [DataRow("http://www.mangareader.net/akame-ga-kiru/1", DisplayName = "'Akame ga Kill!' ch1 pages")]
        [DataRow("http://www.mangareader.net/akame-ga-kiru/7", DisplayName = "'Akame ga Kill!' ch7 pages")]
        [DataRow("http://www.mangareader.net/nisekoi/1", DisplayName = "'Nisekoi' ch1 pages")]
        [DataRow("http://www.mangareader.net/nisekoi/44", DisplayName = "'Nisekoi' ch44 pages")]
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
                Assert.AreEqual(data[i].Url, result[i].Url);
                Assert.AreEqual(data[i].Value, result[i].Value);
            }
        }

        #endregion Async

        #endregion GetPages
    }
}