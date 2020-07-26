using HtmlAgilityPack;
using MangaParser.Core.Exceptions;
using MangaParser.Core.Interfaces;
using MangaParser.Core.Models;
using MangaParser.Parsers.HtmlWebParsers;
using MangaParser.Parsers.Tests.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MangaParser.Parsers.Tests.HtmlWebParsers
{
    [TestClass]
    [TestClassCategory("Parser", "CoreParser")]
    public class CoreParserTests : TestCore
    {
        protected override IParser GetNewParser => new CoreParserFake("https://www.w3.org/");

        #region Constructor

        [DataTestMethod]
        [TestMethodCategory("Constructor")]
        [DataRow("https://readmanga.live/", DisplayName = "'readmanga' url")]
        [DataRow("https://mintmanga.live/", DisplayName = "'mintmanga' url")]
        [DataRow("https://fanfox.net/", DisplayName = "'fanfox' url")]
        [DataRow("http://www.mangareader.net/", DisplayName = "'mangareader' url")]
        public void Constructor_WithUrlString_ShouldSet_Properties(string url)
        {
            // arrange
            Uri uri = new Uri(url);

            // act
            CoreParserFake parser = new CoreParserFake(url);

            // assert
            Assert.IsNotNull(parser.BaseUrl);
            Assert.AreEqual(uri.Host, parser.BaseUrl.Host);
            Assert.IsNotNull(parser.Web);
        }

        [DataTestMethod]
        [TestMethodCategory("Constructor")]
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
        [TestMethodCategory("Constructor")]
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
        [TestMethodCategory("Constructor")]
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
        [TestMethodCategory("GetManga")]
        [TestMethodDelay()]
        [DataRow("https://www.w3.org/Security/", DisplayName = "test page #1 url")]
        [DataRow("https://www.w3.org/2016/09/TPAC/", DisplayName = "test page #2 url")]
        [DataRow("http://www.w3.org/Security/", DisplayName = "test page with http url")]
        public virtual void GetManga_ByUrlString_ShouldReturn_CorrectData(string url)
        {
            // arrange
            CoreParserFake parser = GetNewParser as CoreParserFake;

            MangaObject manga = new MangaObject();
            HtmlDocument dataDoc = new HtmlWeb().Load(url);

            // act
            var result = parser.GetManga(url);
            var Url = parser.Url;
            var doc = parser.HtmlDoc;

            // assert
            Assert.AreEqual(manga, result);
            Assert.AreEqual(url, Url.OriginalString);
            Assert.AreEqual(dataDoc.Text, doc.Text);
        }

        // TODO: try DynamicData
        [DataTestMethod]
        [TestMethodCategory("GetManga")]
        [TestMethodDelay()]
        [DataRow("https://www.w3.org/Security/", DisplayName = "test page #1 url")]
        [DataRow("https://www.w3.org/2016/09/TPAC/", DisplayName = "test page #2 url")]
        [DataRow("http://www.w3.org/Security/", DisplayName = "test page with http url")]
        public virtual void GetManga_ByUri_ShouldReturn_CorrectData(string url)
        {
            // arrange
            CoreParserFake parser = GetNewParser as CoreParserFake;

            Uri uri = new Uri(url);

            MangaObject manga = new MangaObject();
            HtmlDocument dataDoc = new HtmlWeb().Load(uri);

            // act
            var result = parser.GetManga(uri);
            var Url = parser.Url;
            var doc = parser.HtmlDoc;

            // assert
            Assert.AreEqual(manga, result);
            Assert.AreEqual(uri, Url);
            Assert.AreEqual(dataDoc.Text, doc.Text);
        }

        [DataTestMethod]
        [TestMethodCategory("GetManga")]
        [TestMethodDelay()]
        [DataRow("https://www.w3.org/Security/", DisplayName = "test page #1 url")]
        [DataRow("https://www.w3.org/2016/09/TPAC/", DisplayName = "test page #2 url")]
        [DataRow("http://www.w3.org/Security/", DisplayName = "test page with http url")]
        public virtual void GetManga_ByMangaObject_ShouldReturn_CorrectData(string url)
        {
            // arrange
            CoreParserFake parser = GetNewParser as CoreParserFake;

            IMangaObject mangaObject = new MangaObject(null, url);

            MangaObject manga = new MangaObject();
            HtmlDocument dataDoc = new HtmlWeb().Load(mangaObject.Url);

            // act
            var result = parser.GetManga(mangaObject);
            var Url = parser.Url;
            var doc = parser.HtmlDoc;

            // assert
            Assert.AreEqual(manga, result);
            Assert.AreEqual(mangaObject.Url, Url);
            Assert.AreEqual(dataDoc.Text, doc.Text);
        }

        [DataTestMethod]
        [TestMethodCategory("GetManga")]
        [DataRow("asdfasedfqewqfeqwfq", DisplayName = "Rubbish url")]
        [DataRow("", DisplayName = "Empty url")]
        [DataRow(null, DisplayName = "Null url")]
        public virtual void GetManga_ByWrongUrlString_ShouldThrow_UriFormatException(string url)
        {
            // arrange
                        IParser parser = GetNewParser;
            Action action = new Action(() => parser.GetManga(url));

            // act
            // assert
            Assert.ThrowsException<UriFormatException>(action);
        }

        [DataTestMethod]
        [TestMethodCategory("GetManga")]
        public virtual void GetManga_ByNullUri_ShouldThrow_ArgumentNullException()
        {
            // arrange
            Uri uri = null;

                        IParser parser = GetNewParser;
            Action action = new Action(() => parser.GetManga(uri));

            // act
            // assert
            Assert.ThrowsException<ArgumentNullException>(action);
        }

        [DataTestMethod]
        [TestMethodCategory("GetManga")]
        [DataRow("asdfasedfqewqfeqwfq", DisplayName = "Rubbish url")]
        [DataRow("", DisplayName = "Empty url")]
        [DataRow(null, DisplayName = "Null url")]
        public virtual void GetManga_ByWrongMangaObject_ShouldThrow_ArgumentNullException(string url)
        {
            // arrange
            IMangaObject manga = new MangaObject(null, url);

                        IParser parser = GetNewParser;
            Action action = new Action(() => parser.GetManga(manga));

            // act
            // assert
            Assert.ThrowsException<ArgumentNullException>(action);
        }

        [DataTestMethod]
        [TestMethodCategory("GetManga")]
        [DataRow("http://example.com/", DisplayName = "Wrong host url")]
        public virtual void GetManga_ByWrongUrlStringHost_ShouldThrow_BaseHostNotMatchException(string url)
        {
            // arrange
                        IParser parser = GetNewParser;
            Action action = new Action(() => parser.GetManga(url));

            // act
            // assert
            Assert.ThrowsException<BaseHostNotMatchException>(action);
        }

        [DataTestMethod]
        [TestMethodCategory("GetManga")]
        [DataRow("http://example.com/", DisplayName = "Wrong host url")]
        public virtual void GetManga_ByWrongUriHost_ShouldThrow_BaseHostNotMatchException(string url)
        {
            // arrange
            Uri uri = new Uri(url);

                        IParser parser = GetNewParser;
            Action action = new Action(() => parser.GetManga(uri));

            // act
            // assert
            Assert.ThrowsException<BaseHostNotMatchException>(action);
        }

        [DataTestMethod]
        [TestMethodCategory("GetManga")]
        [DataRow("http://example.com/", DisplayName = "Wrong host url")]
        public virtual void GetManga_ByWrongMangaObjectHost_ShouldThrow_BaseHostNotMatchException(string url)
        {
            // arrange
            IMangaObject manga = new MangaObject(null, url);

                        IParser parser = GetNewParser;
            Action action = new Action(() => parser.GetManga(manga));

            // act
            // assert
            Assert.ThrowsException<BaseHostNotMatchException>(action);
        }

        #endregion Sync

        #region Async

        [DataTestMethod]
        [TestMethodCategory("GetManga", "Async")]
        [TestMethodDelay()]
        [DataRow("https://www.w3.org/Security/", DisplayName = "test page #1 url")]
        [DataRow("https://www.w3.org/2016/09/TPAC/", DisplayName = "test page #2 url")]
        [DataRow("http://www.w3.org/Security/", DisplayName = "test page with http url")]
        public virtual async Task GetMangaAsync_ByUrlString_ShouldReturn_CorrectData(string url)
        {
            // arrange
            CoreParserFake parser = GetNewParser as CoreParserFake;

            MangaObject manga = new MangaObject();
            HtmlDocument dataDoc = new HtmlWeb().Load(url);

            // act
            var result = await parser.GetMangaAsync(url);
            var Url = parser.Url;
            var doc = parser.HtmlDoc;

            // assert
            Assert.AreEqual(manga, result);
            Assert.AreEqual(url, Url.OriginalString);
            Assert.AreEqual(dataDoc.Text, doc.Text);
        }

        [DataTestMethod]
        [TestMethodCategory("GetManga", "Async")]
        [TestMethodDelay()]
        [DataRow("https://www.w3.org/Security/", DisplayName = "test page #1 url")]
        [DataRow("https://www.w3.org/2016/09/TPAC/", DisplayName = "test page #2 url")]
        [DataRow("http://www.w3.org/Security/", DisplayName = "test page with http url")]
        public virtual async Task GetMangaAsync_ByUri_ShouldReturn_CorrectData(string url)
        {
            // arrange
            CoreParserFake parser = GetNewParser as CoreParserFake;

            Uri uri = new Uri(url);

            MangaObject manga = new MangaObject();
            HtmlDocument dataDoc = new HtmlWeb().Load(uri);

            // act
            var result = await parser.GetMangaAsync(uri);
            var Url = parser.Url;
            var doc = parser.HtmlDoc;

            // assert
            Assert.AreEqual(manga, result);
            Assert.AreEqual(uri, Url);
            Assert.AreEqual(dataDoc.Text, doc.Text);
        }

        [DataTestMethod]
        [TestMethodCategory("GetManga", "Async")]
        [TestMethodDelay()]
        [DataRow("https://www.w3.org/Security/", DisplayName = "test page #1 url")]
        [DataRow("https://www.w3.org/2016/09/TPAC/", DisplayName = "test page #2 url")]
        [DataRow("http://www.w3.org/Security/", DisplayName = "test page with http url")]
        public virtual async Task GetMangaAsync_ByMangaObject_ShouldReturn_CorrectData(string url)
        {
            // arrange
            CoreParserFake parser = GetNewParser as CoreParserFake;

            IMangaObject mangaObject = new MangaObject(null, url);

            MangaObject manga = new MangaObject();
            HtmlDocument dataDoc = new HtmlWeb().Load(mangaObject.Url);

            // act
            var result = await parser.GetMangaAsync(mangaObject);
            var Url = parser.Url;
            var doc = parser.HtmlDoc;

            // assert
            Assert.AreEqual(manga, result);
            Assert.AreEqual(mangaObject.Url, Url);
            Assert.AreEqual(dataDoc.Text, doc.Text);
        }

        [DataTestMethod]
        [TestMethodCategory("GetManga", "Async")]
        [DataRow("asdfasedfqewqfeqwfq", DisplayName = "Rubbish url")]
        [DataRow("", DisplayName = "Empty url")]
        [DataRow(null, DisplayName = "Null url")]
        public virtual async Task GetMangaAsync_ByWrongUrlString_ShouldThrow_UriFormatException(string url)
        {
            // arrange
                        IParser parser = GetNewParser;
            Task<IMangaObject> task()
            {
                return parser.GetMangaAsync(url);
            }

            // act
            // assert
            await Assert.ThrowsExceptionAsync<UriFormatException>(task);
        }

        [DataTestMethod]
        [TestMethodCategory("GetManga", "Async")]
        [DataRow("asdfasedfqewqfeqwfq", DisplayName = "Rubbish url")]
        [DataRow("", DisplayName = "Empty url")]
        [DataRow(null, DisplayName = "Null url")]
        public virtual async Task GetMangaAsync_ByWrongMangaObject_ShouldThrow_ArgumentNullException(string url)
        {
            // arrange
            IMangaObject manga = new MangaObject(null, url);

                        IParser parser = GetNewParser;
            Task<IMangaObject> task()
            {
                return parser.GetMangaAsync(manga);
            }

            // act
            // assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(task);
        }

        [DataTestMethod]
        [TestMethodCategory("GetManga", "Async")]
        [DataRow("http://example.com/", DisplayName = "Wrong host url")]
        public virtual async Task GetMangaAsync_ByWrongUrlStringHost_ShouldThrow_BaseHostNotMatchException(string url)
        {
            // arrange
                        IParser parser = GetNewParser;
            Task<IMangaObject> task()
            {
                return parser.GetMangaAsync(url);
            }

            // act
            // assert
            await Assert.ThrowsExceptionAsync<BaseHostNotMatchException>(task);
        }

        [DataTestMethod]
        [TestMethodCategory("GetManga", "Async")]
        [DataRow("http://example.com/", DisplayName = "Wrong host url")]
        public virtual async Task GetMangaAsync_ByWrongUriHost_ShouldThrow_BaseHostNotMatchException(string url)
        {
            // arrange
            Uri uri = new Uri(url);

                        IParser parser = GetNewParser;
            Task<IMangaObject> task()
            {
                return parser.GetMangaAsync(uri);
            }

            // act
            // assert
            await Assert.ThrowsExceptionAsync<BaseHostNotMatchException>(task);
        }

        [DataTestMethod]
        [TestMethodCategory("GetManga", "Async")]
        [DataRow("http://example.com/", DisplayName = "Wrong host url")]
        public virtual async Task GetMangaAsync_ByWrongMangaObjectHost_ShouldThrow_BaseHostNotMatchException(string url)
        {
            // arrange
            IMangaObject manga = new MangaObject(null, url);

                        IParser parser = GetNewParser;
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
        [TestMethodCategory("GetChapters")]
        [TestMethodDelay()]
        [DataRow("https://www.w3.org/Security/", DisplayName = "test page #1 url")]
        [DataRow("https://www.w3.org/2016/09/TPAC/", DisplayName = "test page #2 url")]
        [DataRow("http://www.w3.org/Security/", DisplayName = "test page with http url")]
        public virtual void GetChapters_ByUrlString_ShouldReturn_CorrectData(string url)
        {
            // arrange
            CoreParserFake parser = GetNewParser as CoreParserFake;

            var data = Array.Empty<IChapter>();
            HtmlDocument dataDoc = new HtmlWeb().Load(url);

            // act
            var result = parser.GetChapters(url);
            var Url = parser.Url;
            var doc = parser.HtmlDoc;

            // assert
            CollectionAssert.AreEquivalent(data, result.ToList());
            Assert.AreEqual(url, Url.OriginalString);
            Assert.AreEqual(dataDoc.Text, doc.Text);
        }

        [DataTestMethod]
        [TestMethodCategory("GetChapters")]
        [TestMethodDelay()]
        [DataRow("https://www.w3.org/Security/", DisplayName = "test page #1 url")]
        [DataRow("https://www.w3.org/2016/09/TPAC/", DisplayName = "test page #2 url")]
        [DataRow("http://www.w3.org/Security/", DisplayName = "test page with http url")]
        public virtual void GetChapters_ByUri_ShouldReturn_CorrectData(string url)
        {
            // arrange
            CoreParserFake parser = GetNewParser as CoreParserFake;

            Uri uri = new Uri(url);

            var data = Array.Empty<IChapter>();
            HtmlDocument dataDoc = new HtmlWeb().Load(uri);

            // act
            var result = parser.GetChapters(uri);
            var Url = parser.Url;
            var doc = parser.HtmlDoc;

            // assert
            CollectionAssert.AreEquivalent(data, result.ToList());
            Assert.AreEqual(uri, Url);
            Assert.AreEqual(dataDoc.Text, doc.Text);
        }

        [DataTestMethod]
        [TestMethodCategory("GetChapters")]
        [TestMethodDelay()]
        [DataRow("https://www.w3.org/Security/", DisplayName = "test page #1 url")]
        [DataRow("https://www.w3.org/2016/09/TPAC/", DisplayName = "test page #2 url")]
        [DataRow("http://www.w3.org/Security/", DisplayName = "test page with http url")]
        public virtual void GetChapters_ByMangaObject_ShouldReturn_CorrectData(string url)
        {
            // arrange
            CoreParserFake parser = GetNewParser as CoreParserFake;

            IMangaObject mangaObject = new MangaObject(null, url);

            var data = Array.Empty<IChapter>();
            HtmlDocument dataDoc = new HtmlWeb().Load(mangaObject.Url);

            // act
            var result = parser.GetChapters(mangaObject);
            var Url = parser.Url;
            var doc = parser.HtmlDoc;

            // assert
            CollectionAssert.AreEquivalent(data, result.ToList());
            Assert.AreEqual(mangaObject.Url, Url);
            Assert.AreEqual(dataDoc.Text, doc.Text);
        }

        [DataTestMethod]
        [TestMethodCategory("GetChapters")]
        [DataRow("asdfasedfqewqfeqwfq", DisplayName = "Rubbish url")]
        [DataRow("", DisplayName = "Empty url")]
        [DataRow(null, DisplayName = "Null url")]
        public virtual void GetChapters_ByWrongUrlString_ShouldThrow_UriFormatException(string url)
        {
            // arrange
                        IParser parser = GetNewParser;
            Action action = new Action(() => parser.GetChapters(url));

            // act
            // assert
            Assert.ThrowsException<UriFormatException>(action);
        }

        [DataTestMethod]
        [TestMethodCategory("GetChapters")]
        [DataRow("asdfasedfqewqfeqwfq", DisplayName = "Rubbish url")]
        [DataRow("", DisplayName = "Empty url")]
        [DataRow(null, DisplayName = "Null url")]
        public virtual void GetChapters_ByWrongMangaObjecttUrl_ShouldThrow_ArgumentNullException(string url)
        {
            // arrange
            IMangaObject manga = new MangaObject(null, url);

                        IParser parser = GetNewParser;
            Action action = new Action(() => parser.GetChapters(manga));

            // act
            // assert
            Assert.ThrowsException<ArgumentNullException>(action);
        }

        [DataTestMethod]
        [TestMethodCategory("GetChapters")]
        [DataRow("http://example.com/", DisplayName = "Wrong host url")]
        public virtual void GetChapters_ByWrongUrlStringHost_ShouldThrow_BaseHostNotMatchException(string url)
        {
            // arrange
                        IParser parser = GetNewParser;
            Action action = new Action(() => parser.GetChapters(url));

            // act
            // assert
            Assert.ThrowsException<BaseHostNotMatchException>(action);
        }

        [DataTestMethod]
        [TestMethodCategory("GetChapters")]
        [DataRow("http://example.com/", DisplayName = "Wrong host url")]
        public virtual void GetChapters_ByWrongUriHost_ShouldThrow_BaseHostNotMatchException(string url)
        {
            // arrange
            Uri uri = new Uri(url);

                        IParser parser = GetNewParser;
            Action action = new Action(() => parser.GetChapters(uri));

            // act
            // assert
            Assert.ThrowsException<BaseHostNotMatchException>(action);
        }

        [DataTestMethod]
        [TestMethodCategory("GetChapters")]
        [DataRow("http://example.com/", DisplayName = "Wrong host url")]
        public virtual void GetChapters_ByWrongMangaObjectUrltHost_ShouldThrow_BaseHostNotMatchException(string url)
        {
            // arrange
            IMangaObject manga = new MangaObject(null, url);

                        IParser parser = GetNewParser;
            Action action = new Action(() => parser.GetChapters(manga));

            // act
            // assert
            Assert.ThrowsException<BaseHostNotMatchException>(action);
        }

        #endregion Sync

        #region Async

        [DataTestMethod]
        [TestMethodCategory("GetChapters", "Async")]
        [TestMethodDelay()]
        [DataRow("https://www.w3.org/Security/", DisplayName = "test page #1 url")]
        [DataRow("https://www.w3.org/2016/09/TPAC/", DisplayName = "test page #2 url")]
        [DataRow("http://www.w3.org/Security/", DisplayName = "test page with http url")]
        public virtual async Task GetChaptersAsync_ByUrlString_ShouldReturn_CorrectData(string url)
        {
            // arrange
            CoreParserFake parser = GetNewParser as CoreParserFake;

            var data = Array.Empty<IChapter>();
            HtmlDocument dataDoc = new HtmlWeb().Load(url);

            // act
            var result = await parser.GetChaptersAsync(url);
            var Url = parser.Url;
            var doc = parser.HtmlDoc;

            // assert
            CollectionAssert.AreEquivalent(data, result.ToList());
            Assert.AreEqual(url, Url.OriginalString);
            Assert.AreEqual(dataDoc.Text, doc.Text);
        }

        [DataTestMethod]
        [TestMethodCategory("GetChapters", "Async")]
        [TestMethodDelay()]
        [DataRow("https://www.w3.org/Security/", DisplayName = "test page #1 url")]
        [DataRow("https://www.w3.org/2016/09/TPAC/", DisplayName = "test page #2 url")]
        [DataRow("http://www.w3.org/Security/", DisplayName = "test page with http url")]
        public virtual async Task GetChaptersAsync_ByUri_ShouldReturn_CorrectData(string url)
        {
            // arrange
            CoreParserFake parser = GetNewParser as CoreParserFake;

            Uri uri = new Uri(url);

            var data = Array.Empty<IChapter>();
            HtmlDocument dataDoc = new HtmlWeb().Load(uri);

            // act
            var result = await parser.GetChaptersAsync(uri);
            var Url = parser.Url;
            var doc = parser.HtmlDoc;

            // assert
            CollectionAssert.AreEquivalent(data, result.ToList());
            Assert.AreEqual(uri, Url);
            Assert.AreEqual(dataDoc.Text, doc.Text);
        }

        [DataTestMethod]
        [TestMethodCategory("GetChapters", "Async")]
        [TestMethodDelay()]
        [DataRow("https://www.w3.org/Security/", DisplayName = "test page #1 url")]
        [DataRow("https://www.w3.org/2016/09/TPAC/", DisplayName = "test page #2 url")]
        [DataRow("http://www.w3.org/Security/", DisplayName = "test page with http url")]
        public virtual async Task GetChaptersAsync_ByMangaObject_ShouldReturn_CorrectData(string url)
        {
            // arrange
            CoreParserFake parser = GetNewParser as CoreParserFake;

            IMangaObject mangaObject = new MangaObject(null, url);

            var data = Array.Empty<IChapter>();
            HtmlDocument dataDoc = new HtmlWeb().Load(mangaObject.Url);

            // act
            var result = await parser.GetChaptersAsync(mangaObject);
            var Url = parser.Url;
            var doc = parser.HtmlDoc;

            // assert
            CollectionAssert.AreEquivalent(data, result.ToList());
            Assert.AreEqual(mangaObject.Url, Url);
            Assert.AreEqual(dataDoc.Text, doc.Text);
        }

        [DataTestMethod]
        [TestMethodCategory("GetChapters", "Async")]
        [DataRow("asdfasedfqewqfeqwfq", DisplayName = "Rubbish url")]
        [DataRow("", DisplayName = "Empty url")]
        [DataRow(null, DisplayName = "Null url")]
        public virtual async Task GetChaptersAsync_ByWrongUrlString_ShouldThrow_UriFormatException(string url)
        {
            // arrange
                        IParser parser = GetNewParser;

            Task<IEnumerable<IChapter>> task()
            {
                return parser.GetChaptersAsync(url);
            }

            // act
            // assert
            await Assert.ThrowsExceptionAsync<UriFormatException>(task);
        }

        [DataTestMethod]
        [TestMethodCategory("GetChapters", "Async")]
        [DataRow("asdfasedfqewqfeqwfq", DisplayName = "Rubbish url")]
        [DataRow("", DisplayName = "Empty url")]
        [DataRow(null, DisplayName = "Null url")]
        public virtual async Task GetChaptersAsync_ByWrongMangaObjectUrl_ShouldThrow_ArgumentNullException(string url)
        {
            // arrange
            IMangaObject manga = new MangaObject(null, url);

                        IParser parser = GetNewParser;

            Task<IEnumerable<IChapter>> task()
            {
                return parser.GetChaptersAsync(manga);
            }

            // act
            // assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(task);
        }

        [DataTestMethod]
        [TestMethodCategory("GetChapters", "Async")]
        [DataRow("http://example.com/", DisplayName = "Wrong host url")]
        public virtual async Task GetChaptersAsync_ByWrongUrlStringHost_ShouldThrow_BaseHostNotMatchException(string url)
        {
            // arrange
                        IParser parser = GetNewParser;

            Task<IEnumerable<IChapter>> task()
            {
                return parser.GetChaptersAsync(url);
            }

            // act
            // assert
            await Assert.ThrowsExceptionAsync<BaseHostNotMatchException>(task);
        }

        [DataTestMethod]
        [TestMethodCategory("GetChapters", "Async")]
        [DataRow("http://example.com/", DisplayName = "Wrong host url")]
        public virtual async Task GetChaptersAsync_ByWrongUriHost_ShouldThrow_BaseHostNotMatchException(string url)
        {
            // arrange
            Uri uri = new Uri(url);

                        IParser parser = GetNewParser;

            Task<IEnumerable<IChapter>> task()
            {
                return parser.GetChaptersAsync(uri);
            }

            // act
            // assert
            await Assert.ThrowsExceptionAsync<BaseHostNotMatchException>(task);
        }

        [DataTestMethod]
        [TestMethodCategory("GetChapters", "Async")]
        [DataRow("http://example.com/", DisplayName = "Wrong host url")]
        public virtual async Task GetChaptersAsync_ByWrongMangaObjectUrltHost_ShouldThrow_BaseHostNotMatchException(string url)
        {
            // arrange
            IMangaObject manga = new MangaObject(null, url);

                        IParser parser = GetNewParser;

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
        [TestMethodCategory("GetPages")]
        [TestMethodDelay()]
        [DataRow("https://www.w3.org/Security/", DisplayName = "test page #1 url")]
        [DataRow("https://www.w3.org/2016/09/TPAC/", DisplayName = "test page #2 url")]
        [DataRow("http://www.w3.org/Security/", DisplayName = "test page with http url")]
        public virtual void GetPages_ByUrlString_ShouldReturn_CorrectData(string url)
        {
            // arrange
            CoreParserFake parser = GetNewParser as CoreParserFake;

            var data = Array.Empty<IDataBase>();
            HtmlDocument dataDoc = new HtmlWeb().Load(url);

            // act
            var result = parser.GetPages(url);
            var Url = parser.Url;
            var doc = parser.HtmlDoc;

            // assert
            CollectionAssert.AreEquivalent(data, result.ToList());
            Assert.AreEqual(url, Url.OriginalString);
            Assert.AreEqual(dataDoc.Text, doc.Text);
        }

        [DataTestMethod]
        [TestMethodCategory("GetPages")]
        [TestMethodDelay()]
        [DataRow("https://www.w3.org/Security/", DisplayName = "test page #1 url")]
        [DataRow("https://www.w3.org/2016/09/TPAC/", DisplayName = "test page #2 url")]
        [DataRow("http://www.w3.org/Security/", DisplayName = "test page with http url")]
        public virtual void GetPages_ByUri_ShouldReturn_CorrectData(string url)
        {
            // arrange
            CoreParserFake parser = GetNewParser as CoreParserFake;

            Uri uri = new Uri(url);

            var data = Array.Empty<IDataBase>();
            HtmlDocument dataDoc = new HtmlWeb().Load(uri);

            // act
            var result = parser.GetPages(uri);
            var Url = parser.Url;
            var doc = parser.HtmlDoc;

            // assert
            CollectionAssert.AreEquivalent(data, result.ToList());
            Assert.AreEqual(uri, Url);
            Assert.AreEqual(dataDoc.Text, doc.Text);
        }

        [DataTestMethod]
        [TestMethodCategory("GetPages")]
        [TestMethodDelay()]
        [DataRow("https://www.w3.org/Security/", DisplayName = "test page #1 url")]
        [DataRow("https://www.w3.org/2016/09/TPAC/", DisplayName = "test page #2 url")]
        [DataRow("http://www.w3.org/Security/", DisplayName = "test page with http url")]
        public virtual void GetPages_ByChapter_ShouldReturn_CorrectData(string url)
        {
            // arrange
            CoreParserFake parser = GetNewParser as CoreParserFake;

            IChapter mangaObject = new Chapter(null, url, DateTime.Now);

            var data = Array.Empty<IDataBase>();
            HtmlDocument dataDoc = new HtmlWeb().Load(mangaObject.Url);

            // act
            var result = parser.GetPages(mangaObject);
            var Url = parser.Url;
            var doc = parser.HtmlDoc;

            // assert
            CollectionAssert.AreEquivalent(data, result.ToList());
            Assert.AreEqual(mangaObject.Url, Url);
            Assert.AreEqual(dataDoc.Text, doc.Text);
        }

        [DataTestMethod]
        [TestMethodCategory("GetPages")]
        [DataRow("asdfasedfqewqfeqwfq", DisplayName = "Rubbish url")]
        [DataRow("", DisplayName = "Empty url")]
        [DataRow(null, DisplayName = "Null url")]
        public virtual void GetPages_ByWrongUrlString_ShouldThrow_UriFormatException(string url)
        {
            // arrange
                        IParser parser = GetNewParser;
            Action action = new Action(() => parser.GetPages(url));

            // act
            // assert
            Assert.ThrowsException<UriFormatException>(action);
        }

        [DataTestMethod]
        [TestMethodCategory("GetPages")]
        [DataRow("asdfasedfqewqfeqwfq", DisplayName = "Rubbish url")]
        [DataRow("", DisplayName = "Empty url")]
        [DataRow(null, DisplayName = "Null url")]
        public virtual void GetPages_ByWrongChapterObjectUrl_ShouldThrow_ArgumentNullException(string url)
        {
            // arrange
            IChapter chapter = new Chapter(null, url, DateTime.Now);

                        IParser parser = GetNewParser;
            Action action = new Action(() => parser.GetPages(chapter));

            // act
            // assert
            Assert.ThrowsException<ArgumentNullException>(action);
        }

        [DataTestMethod]
        [TestMethodCategory("GetPages")]
        [DataRow("http://example.com/", DisplayName = "Wrong host url")]
        public virtual void GetPages_ByWrongUrlStringHost_ShouldThrow_BaseHostNotMatchException(string url)
        {
            // arrange
                        IParser parser = GetNewParser;
            Action action = new Action(() => parser.GetPages(url));

            // act
            // assert
            Assert.ThrowsException<BaseHostNotMatchException>(action);
        }

        [DataTestMethod]
        [TestMethodCategory("GetPages")]
        [DataRow("http://example.com/", DisplayName = "Wrong host url")]
        public virtual void GetPages_ByWrongUriHost_ShouldThrow_BaseHostNotMatchException(string url)
        {
            // arrange
            Uri uri = new Uri(url);

                        IParser parser = GetNewParser;
            Action action = new Action(() => parser.GetPages(uri));

            // act
            // assert
            Assert.ThrowsException<BaseHostNotMatchException>(action);
        }

        [DataTestMethod]
        [TestMethodCategory("GetPages")]
        [DataRow("http://example.com/", DisplayName = "Wrong host url")]
        public virtual void GetPages_ByWrongChapterUrltHost_ShouldThrow_BaseHostNotMatchException(string url)
        {
            // arrange
            IChapter manga = new Chapter(null, url, DateTime.Now);

                        IParser parser = GetNewParser;
            Action action = new Action(() => parser.GetPages(manga));

            // act
            // assert
            Assert.ThrowsException<BaseHostNotMatchException>(action);
        }

        #endregion Sync

        #region Async

        [DataTestMethod]
        [TestMethodCategory("GetPages", "Async")]
        [TestMethodDelay()]
        [DataRow("https://www.w3.org/Security/", DisplayName = "test page #1 url")]
        [DataRow("https://www.w3.org/2016/09/TPAC/", DisplayName = "test page #2 url")]
        [DataRow("http://www.w3.org/Security/", DisplayName = "test page with http url")]
        public virtual async Task GetPagesAsync_ByUrlString_ShouldReturn_CorrectData(string url)
        {
            // arrange
            CoreParserFake parser = GetNewParser as CoreParserFake;

            var data = Array.Empty<IDataBase>();
            HtmlDocument dataDoc = new HtmlWeb().Load(url);

            // act
            var result = await parser.GetPagesAsync(url);
            var Url = parser.Url;
            var doc = parser.HtmlDoc;

            // assert
            CollectionAssert.AreEquivalent(data, result.ToList());
            Assert.AreEqual(url, Url.OriginalString);
            Assert.AreEqual(dataDoc.Text, doc.Text);
        }

        [DataTestMethod]
        [TestMethodCategory("GetPages", "Async")]
        [TestMethodDelay()]
        [DataRow("https://www.w3.org/Security/", DisplayName = "test page #1 url")]
        [DataRow("https://www.w3.org/2016/09/TPAC/", DisplayName = "test page #2 url")]
        [DataRow("http://www.w3.org/Security/", DisplayName = "test page with http url")]
        public virtual async Task GetPagesAsync_ByUri_ShouldReturn_CorrectData(string url)
        {
            // arrange
            CoreParserFake parser = GetNewParser as CoreParserFake;

            Uri uri = new Uri(url);

            var data = Array.Empty<IDataBase>();
            HtmlDocument dataDoc = new HtmlWeb().Load(uri);

            // act
            var result = await parser.GetPagesAsync(uri);
            var Url = parser.Url;
            var doc = parser.HtmlDoc;

            // assert
            CollectionAssert.AreEquivalent(data, result.ToList());
            Assert.AreEqual(uri, Url);
            Assert.AreEqual(dataDoc.Text, doc.Text);
        }

        [DataTestMethod]
        [TestMethodCategory("GetPages", "Async")]
        [TestMethodDelay()]
        [DataRow("https://www.w3.org/Security/", DisplayName = "test page #1 url")]
        [DataRow("https://www.w3.org/2016/09/TPAC/", DisplayName = "test page #2 url")]
        [DataRow("http://www.w3.org/Security/", DisplayName = "test page with http url")]
        public virtual async Task GetPagesAsync_ByChapter_ShouldReturn_CorrectData(string url)
        {
            // arrange
            CoreParserFake parser = GetNewParser as CoreParserFake;

            IChapter mangaObject = new Chapter(null, url, DateTime.Now);

            var data = Array.Empty<IDataBase>();
            HtmlDocument dataDoc = new HtmlWeb().Load(mangaObject.Url);

            // act
            var result = await parser.GetPagesAsync(mangaObject);
            var Url = parser.Url;
            var doc = parser.HtmlDoc;

            // assert
            CollectionAssert.AreEquivalent(data, result.ToList());
            Assert.AreEqual(mangaObject.Url, Url);
            Assert.AreEqual(dataDoc.Text, doc.Text);
        }

        [DataTestMethod]
        [TestMethodCategory("GetPages", "Async")]
        [DataRow("asdfasedfqewqfeqwfq", DisplayName = "Rubbish url")]
        [DataRow("", DisplayName = "Empty url")]
        [DataRow(null, DisplayName = "Null url")]
        public virtual async Task GetPagesAsync_ByWrongUrlString_ShouldThrow_UriFormatException(string url)
        {
            // arrange
                        IParser parser = GetNewParser;
            Task<IEnumerable<IDataBase>> task()
            {
                return parser.GetPagesAsync(url);
            }

            // act
            // assert
            await Assert.ThrowsExceptionAsync<UriFormatException>(task);
        }

        [DataTestMethod]
        [TestMethodCategory("GetPages", "Async")]
        [DataRow("asdfasedfqewqfeqwfq", DisplayName = "Rubbish url")]
        [DataRow("", DisplayName = "Empty url")]
        [DataRow(null, DisplayName = "Null url")]
        public virtual async Task GetPagesAsync_ByWrongChapterObjectUrl_ShouldThrow_ArgumentNullException(string url)
        {
            // arrange
            IChapter chapter = new Chapter(null, url, DateTime.Now);

                        IParser parser = GetNewParser;
            Task<IEnumerable<IDataBase>> task()
            {
                return parser.GetPagesAsync(chapter);
            }
            // act
            // assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(task);
        }

        [DataTestMethod]
        [TestMethodCategory("GetPages", "Async")]
        [DataRow("http://example.com/", DisplayName = "Wrong host url")]
        public virtual async Task GetPagesAsync_ByWrongUrlStringHost_ShouldThrow_BaseHostNotMatchException(string url)
        {
            // arrange
                        IParser parser = GetNewParser;
            Task<IEnumerable<IDataBase>> task()
            {
                return parser.GetPagesAsync(url);
            }
            // act
            // assert
            await Assert.ThrowsExceptionAsync<BaseHostNotMatchException>(task);
        }

        [DataTestMethod]
        [TestMethodCategory("GetPages", "Async")]
        [DataRow("http://example.com/", DisplayName = "Wrong host url")]
        public virtual async Task GetPagesAsync_ByWrongUriHost_ShouldThrow_BaseHostNotMatchException(string url)
        {
            // arrange
            Uri uri = new Uri(url);

                        IParser parser = GetNewParser;
            Task<IEnumerable<IDataBase>> task()
            {
                return parser.GetPagesAsync(uri);
            }

            // act
            // assert
            await Assert.ThrowsExceptionAsync<BaseHostNotMatchException>(task);
        }

        [DataTestMethod]
        [TestMethodCategory("GetPages", "Async")]
        [DataRow("http://example.com/", DisplayName = "Wrong host url")]
        public virtual async Task GetPagesAsync_ByWrongChapterUrltHost_ShouldThrow_BaseHostNotMatchException(string url)
        {
            // arrange
            IChapter chapter = new Chapter(null, url, DateTime.Now);

                        IParser parser = GetNewParser;
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
            public HtmlDocument HtmlDoc { get; private set; }
            public Uri Url { get; private set; }

            protected override IEnumerable<IChapter> GetChaptersCore(HtmlDocument htmlDoc, Uri url)
            {
                HtmlDoc = htmlDoc;
                Url = url;
                return Array.Empty<IChapter>();
            }

            protected override IMangaObject GetMangaCore(HtmlDocument htmlDoc, Uri url)
            {
                HtmlDoc = htmlDoc;
                Url = url;
                return new MangaObject();
            }

            protected override IEnumerable<IDataBase> GetPagesCore(HtmlDocument htmlDoc, Uri url)
            {
                HtmlDoc = htmlDoc;
                Url = url;
                return Array.Empty<IDataBase>();
            }

            protected override IEnumerable<IMangaObject> SearchMangaCore(HtmlDocument htmlDoc)
            {
                HtmlDoc = htmlDoc;
                return Array.Empty<IMangaObject>();
            }
        }
    }
}