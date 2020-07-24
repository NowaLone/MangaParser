using HtmlAgilityPack;
using MangaParser.Core.Exceptions;
using MangaParser.Core.Interfaces;
using MangaParser.Core.Models;
using MangaParser.Parsers.HtmlWebParsers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MangaParser.Parsers.Tests.HtmlWebParsers
{
    [TestClass]
    [TestCategory("Parser")]
    [TestCategory("CoreParser")]
    public class CoreParserTests
    {
        protected const string dataPath = "Data";

        protected virtual string GetDataPath(string parserPath, string methodPath, string name, string extension = ".json")
        {
            return Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", dataPath ?? String.Empty, parserPath ?? String.Empty, methodPath ?? String.Empty, name + extension ?? String.Empty));
        }

        #region Constructor

        [DataTestMethod]
        [TestCategory("Constructor")]
        [DataRow("https://readmanga.live/", DisplayName = "'readmanga' url")]
        [DataRow("https://mintmanga.live/", DisplayName = "'mintmanga' url")]
        [DataRow("https://fanfox.net/", DisplayName = "'fanfox' url")]
        [DataRow("http://www.mangareader.net/", DisplayName = "'mangareader' url")]
        public void Constructor_WithUrlString_ShouldSet_Properties(string url)
        {
            // arrange
            // act
            CoreParserFake parser = new CoreParserFake(url);

            // assert
            Assert.IsNotNull(parser.BaseUrl);
            Assert.AreEqual(new Uri(url).Host, parser.BaseUrl.Host);
            Assert.IsNotNull(parser.Web);
        }

        [DataTestMethod]
        [TestCategory("Constructor")]
        [DataRow("https://readmanga.live/", DisplayName = "'readmanga' url")]
        [DataRow("https://mintmanga.live/", DisplayName = "'mintmanga' url")]
        [DataRow("https://fanfox.net/", DisplayName = "'fanfox' url")]
        [DataRow("http://www.mangareader.net/", DisplayName = "'mangareader' url")]
        public void Constructor_WithUri_ShouldSet_Properties(string url)
        {
            // arrange
            Uri uri = new Uri(url);

            // act
            CoreParserFake parser = new CoreParserFake(uri);

            // assert
            Assert.IsNotNull(parser.BaseUrl);
            Assert.AreEqual(uri.Host, parser.BaseUrl.Host);
            Assert.IsNotNull(parser.Web);
        }

        [DataTestMethod]
        [TestCategory("Constructor")]
        [DataRow("asdfasedfqewqfeqwfq", DisplayName = "Rubbish url")]
        [DataRow("", DisplayName = "Empty url")]
        [DataRow(null, DisplayName = "Null url")]
        public void Constructor_WithWrongUrlString_ShouldThrow_UriFormatException(string url)
        {
            // arrange
            // act
            Action action = new Action(() => new CoreParserFake(url));

            // assert
            Assert.ThrowsException<UriFormatException>(action);
        }

        [TestMethod]
        [TestCategory("Constructor")]
        public void Constructor_WithNullUri_ShouldThrow_ArgumentNullException()
        {
            // arrange
            Uri uri = null;

            // act
            Action action = new Action(() => new CoreParserFake(uri));

            // assert
            Assert.ThrowsException<ArgumentNullException>(action);
        }

        #endregion Constructor

        #region GetManga

        #region Sync

        [DataTestMethod]
        [TestCategory("GetManga")]
        [DataRow("asdfasedfqewqfeqwfq", DisplayName = "Rubbish url")]
        [DataRow("", DisplayName = "Empty url")]
        [DataRow(null, DisplayName = "Null url")]
        public void GetManga_ByWrongUrlString_ShouldThrow_UriFormatException(string url)
        {
            // arrange
            CoreParserFake parser = new CoreParserFake("https://www.w3.org/");
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

            CoreParserFake parser = new CoreParserFake("https://www.w3.org/");
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
            CoreParserFake parser = new CoreParserFake("https://www.w3.org/");
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

            CoreParserFake parser = new CoreParserFake("https://www.w3.org/");
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

            CoreParserFake parser = new CoreParserFake("https://www.w3.org/");
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
        [DataRow("asdfasedfqewqfeqwfq", DisplayName = "Rubbish url")]
        [DataRow("", DisplayName = "Empty url")]
        [DataRow(null, DisplayName = "Null url")]
        public async Task GetMangaAsync_ByWrongUrlString_ShouldThrow_UriFormatException(string url)
        {
            // arrange
            CoreParserFake parser = new CoreParserFake("https://www.w3.org/");
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

            CoreParserFake parser = new CoreParserFake("https://www.w3.org/");
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
            CoreParserFake parser = new CoreParserFake("https://www.w3.org/");
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

            CoreParserFake parser = new CoreParserFake("https://www.w3.org/");
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

            CoreParserFake parser = new CoreParserFake("https://www.w3.org/");
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
        [DataRow("asdfasedfqewqfeqwfq", DisplayName = "Rubbish url")]
        [DataRow("", DisplayName = "Empty url")]
        [DataRow(null, DisplayName = "Null url")]
        public void GetChapters_ByWrongUrlString_ShouldThrow_UriFormatException(string url)
        {
            // arrange
            CoreParserFake parser = new CoreParserFake("https://www.w3.org/");
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

            CoreParserFake parser = new CoreParserFake("https://www.w3.org/");
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
            CoreParserFake parser = new CoreParserFake("https://www.w3.org/");
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

            CoreParserFake parser = new CoreParserFake("https://www.w3.org/");
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

            CoreParserFake parser = new CoreParserFake("https://www.w3.org/");
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
        [DataRow("asdfasedfqewqfeqwfq", DisplayName = "Rubbish url")]
        [DataRow("", DisplayName = "Empty url")]
        [DataRow(null, DisplayName = "Null url")]
        public async Task GetChaptersAsync_ByWrongUrlString_ShouldThrow_UriFormatException(string url)
        {
            // arrange
            CoreParserFake parser = new CoreParserFake("https://www.w3.org/");

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

            CoreParserFake parser = new CoreParserFake("https://www.w3.org/");

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
            CoreParserFake parser = new CoreParserFake("https://www.w3.org/");

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

            CoreParserFake parser = new CoreParserFake("https://www.w3.org/");

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

            CoreParserFake parser = new CoreParserFake("https://www.w3.org/");

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
        [DataRow("asdfasedfqewqfeqwfq", DisplayName = "Rubbish url")]
        [DataRow("", DisplayName = "Empty url")]
        [DataRow(null, DisplayName = "Null url")]
        public void GetPages_ByWrongUrlString_ShouldThrow_UriFormatException(string url)
        {
            // arrange
            CoreParserFake parser = new CoreParserFake("https://www.w3.org/");
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

            CoreParserFake parser = new CoreParserFake("https://www.w3.org/");
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
            CoreParserFake parser = new CoreParserFake("https://www.w3.org/");
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

            CoreParserFake parser = new CoreParserFake("https://www.w3.org/");
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

            CoreParserFake parser = new CoreParserFake("https://www.w3.org/");
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
        [DataRow("asdfasedfqewqfeqwfq", DisplayName = "Rubbish url")]
        [DataRow("", DisplayName = "Empty url")]
        [DataRow(null, DisplayName = "Null url")]
        public async Task GetPagesAsync_ByWrongUrlString_ShouldThrow_UriFormatException(string url)
        {
            // arrange
            CoreParserFake parser = new CoreParserFake("https://www.w3.org/");
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

            CoreParserFake parser = new CoreParserFake("https://www.w3.org/");
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
            CoreParserFake parser = new CoreParserFake("https://www.w3.org/");
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

            CoreParserFake parser = new CoreParserFake("https://www.w3.org/");
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

            CoreParserFake parser = new CoreParserFake("https://www.w3.org/");
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

        private class CoreParserFake : CoreParser
        {
            public CoreParserFake(Uri baseUrl) : base(baseUrl)
            {
            }

            public CoreParserFake(string baseUrl) : base(baseUrl)
            {
            }

            public new HtmlWeb Web => base.Web;

            protected override IEnumerable<IChapter> GetChaptersCore(HtmlDocument htmlDoc, Uri url)
            {
                throw new NotImplementedException();
            }

            protected override IMangaObject GetMangaCore(HtmlDocument htmlDoc, Uri url)
            {
                throw new NotImplementedException();
            }

            protected override IEnumerable<IDataBase> GetPagesCore(HtmlDocument htmlDoc, Uri url)
            {
                throw new NotImplementedException();
            }

            protected override IEnumerable<IMangaObject> SearchMangaCore(HtmlDocument htmlDoc)
            {
                throw new NotImplementedException();
            }
        }
    }
}