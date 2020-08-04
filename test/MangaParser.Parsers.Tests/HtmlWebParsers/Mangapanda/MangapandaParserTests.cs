using MangaParser.Core.Interfaces;
using MangaParser.Parsers.HtmlWebParsers.Mangapanda;
using MangaParser.Parsers.Tests.Attributes;
using MangaParser.Parsers.Tests.HtmlWebParsers.Mangareader;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace MangaParser.Parsers.Tests.HtmlWebParsers.Mangapanda
{
    [TestClass]
    [TestClassCategory("Parser", "Mangapanda")]
    public class MangapandaParserTests : MangareaderParserTests
    {
        #region Properties

        protected override string ParserPath => "Mangapanda";
        protected override IParser GetNewParser => new MangapandaParser();

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
        public override void SearchManga_WithQuery_ShouldReturn_CorrectData(string query)
        {
            base.SearchManga_WithQuery_ShouldReturn_CorrectData(query);
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
        public override async Task SearchMangaAsync_WithQuery_ShouldReturn_CorrectData(string query)
        {
            await base.SearchMangaAsync_WithQuery_ShouldReturn_CorrectData(query);
        }

        #endregion Async

        #endregion Search

        #region GetManga

        #region Sync

        [DataTestMethod]
        [TestMethodCategory("GetManga")]
        [TestMethodDelay(5000)]
        [DataRow("http://www.mangapanda.com/sakamoto-desu-ga", DisplayName = "'Sakamoto desu ga?' url")]
        [DataRow("http://www.mangapanda.com/akame-ga-kiru", DisplayName = "'Akame ga Kill!' url")]
        [DataRow("http://www.mangapanda.com/nisekoi", DisplayName = "'Nisekoi' url")]
        [DataRow("http://www.mangapanda.com/onepunch-man", DisplayName = "'Onepunch-Man' url")]
        public override void GetManga_ByUrlString_ShouldReturn_CorrectData(string url)
        {
            base.GetManga_ByUrlString_ShouldReturn_CorrectData(url);
        }

        [DataTestMethod]
        [TestMethodCategory("GetManga")]
        [TestMethodDelay(5000)]
        [DataRow("http://www.mangapanda.com/sakamoto-desu-ga", DisplayName = "'Sakamoto desu ga?' url")]
        [DataRow("http://www.mangapanda.com/akame-ga-kiru", DisplayName = "'Akame ga Kill!' url")]
        [DataRow("http://www.mangapanda.com/nisekoi", DisplayName = "'Nisekoi' url")]
        [DataRow("http://www.mangapanda.com/onepunch-man", DisplayName = "'Onepunch-Man' url")]
        public override void GetManga_ByUri_ShouldReturn_CorrectData(string url)
        {
            base.GetManga_ByUri_ShouldReturn_CorrectData(url);
        }

        [DataTestMethod]
        [TestMethodCategory("GetManga")]
        [TestMethodDelay(5000)]
        [DataRow("http://www.mangapanda.com/sakamoto-desu-ga", DisplayName = "'Sakamoto desu ga?' url")]
        [DataRow("http://www.mangapanda.com/akame-ga-kiru", DisplayName = "'Akame ga Kill!' url")]
        [DataRow("http://www.mangapanda.com/nisekoi", DisplayName = "'Nisekoi' url")]
        [DataRow("http://www.mangapanda.com/onepunch-man", DisplayName = "'Onepunch-Man' url")]
        public override void GetManga_ByMangaObject_ShouldReturn_CorrectData(string url)
        {
            base.GetManga_ByMangaObject_ShouldReturn_CorrectData(url);
        }

        #endregion Sync

        #region Async

        [DataTestMethod]
        [TestMethodCategory("GetManga", "Async")]
        [TestMethodDelay(5000)]
        [DataRow("http://www.mangapanda.com/sakamoto-desu-ga", DisplayName = "'Sakamoto desu ga?' url")]
        [DataRow("http://www.mangapanda.com/akame-ga-kiru", DisplayName = "'Akame ga Kill!' url")]
        [DataRow("http://www.mangapanda.com/nisekoi", DisplayName = "'Nisekoi' url")]
        [DataRow("http://www.mangapanda.com/onepunch-man", DisplayName = "'Onepunch-Man' url")]
        public override async Task GetMangaAsync_ByUrlString_ShouldReturn_CorrectData(string url)
        {
            await base.GetMangaAsync_ByUrlString_ShouldReturn_CorrectData(url);
        }

        [DataTestMethod]
        [TestMethodCategory("GetManga", "Async")]
        [TestMethodDelay(5000)]
        [DataRow("http://www.mangapanda.com/sakamoto-desu-ga", DisplayName = "'Sakamoto desu ga?' url")]
        [DataRow("http://www.mangapanda.com/akame-ga-kiru", DisplayName = "'Akame ga Kill!' url")]
        [DataRow("http://www.mangapanda.com/nisekoi", DisplayName = "'Nisekoi' url")]
        [DataRow("http://www.mangapanda.com/onepunch-man", DisplayName = "'Onepunch-Man' url")]
        public override async Task GetMangaAsync_ByUri_ShouldReturn_CorrectData(string url)
        {
            await base.GetMangaAsync_ByUri_ShouldReturn_CorrectData(url);
        }

        [DataTestMethod]
        [TestMethodCategory("GetManga", "Async")]
        [TestMethodDelay(5000)]
        [DataRow("http://www.mangapanda.com/sakamoto-desu-ga", DisplayName = "'Sakamoto desu ga?' url")]
        [DataRow("http://www.mangapanda.com/akame-ga-kiru", DisplayName = "'Akame ga Kill!' url")]
        [DataRow("http://www.mangapanda.com/nisekoi", DisplayName = "'Nisekoi' url")]
        [DataRow("http://www.mangapanda.com/onepunch-man", DisplayName = "'Onepunch-Man' url")]
        public override async Task GetMangaAsync_ByMangaObject_ShouldReturn_CorrectData(string url)
        {
            await base.GetMangaAsync_ByMangaObject_ShouldReturn_CorrectData(url);
        }

        #endregion Async

        #endregion GetManga

        #region GetChapters

        #region Sync

        [DataTestMethod]
        [TestMethodCategory("GetChapters")]
        [TestMethodDelay(5000)]
        [DataRow("http://www.mangapanda.com/sakamoto-desu-ga", DisplayName = "'Sakamoto desu ga?' chapters")]
        [DataRow("http://www.mangapanda.com/akame-ga-kiru", DisplayName = "'Akame ga Kill!' chapters")]
        [DataRow("http://www.mangapanda.com/nisekoi", DisplayName = "'Nisekoi' chapters")]
        public override void GetChapters_ByUrlString_ShouldReturn_CorrectData(string url)
        {
            base.GetChapters_ByUrlString_ShouldReturn_CorrectData(url);
        }

        [DataTestMethod]
        [TestMethodCategory("GetChapters")]
        [TestMethodDelay(5000)]
        [DataRow("http://www.mangapanda.com/sakamoto-desu-ga", DisplayName = "'Sakamoto desu ga?' chapters")]
        [DataRow("http://www.mangapanda.com/akame-ga-kiru", DisplayName = "'Akame ga Kill!' chapters")]
        [DataRow("http://www.mangapanda.com/nisekoi", DisplayName = "'Nisekoi' chapters")]
        public override void GetChapters_ByUri_ShouldReturn_CorrectData(string url)
        {
            base.GetChapters_ByUri_ShouldReturn_CorrectData(url);
        }

        [DataTestMethod]
        [TestMethodCategory("GetChapters")]
        [TestMethodDelay(5000)]
        [DataRow("http://www.mangapanda.com/sakamoto-desu-ga", DisplayName = "'Sakamoto desu ga?' chapters")]
        [DataRow("http://www.mangapanda.com/akame-ga-kiru", DisplayName = "'Akame ga Kill!' chapters")]
        [DataRow("http://www.mangapanda.com/nisekoi", DisplayName = "'Nisekoi' chapters")]
        public override void GetChapters_ByMangaObject_ShouldReturn_CorrectData(string url)
        {
            base.GetChapters_ByMangaObject_ShouldReturn_CorrectData(url);
        }

        #endregion Sync

        #region Async

        [DataTestMethod]
        [TestMethodCategory("GetChapters", "Async")]
        [TestMethodDelay(5000)]
        [DataRow("http://www.mangapanda.com/sakamoto-desu-ga", DisplayName = "'Sakamoto desu ga?' chapters")]
        [DataRow("http://www.mangapanda.com/akame-ga-kiru", DisplayName = "'Akame ga Kill!' chapters")]
        [DataRow("http://www.mangapanda.com/nisekoi", DisplayName = "'Nisekoi' chapters")]
        public override async Task GetChaptersAsync_ByUrlString_ShouldReturn_CorrectData(string url)
        {
            await base.GetChaptersAsync_ByUrlString_ShouldReturn_CorrectData(url);
        }

        [DataTestMethod]
        [TestMethodCategory("GetChapters", "Async")]
        [TestMethodDelay(5000)]
        [DataRow("http://www.mangapanda.com/sakamoto-desu-ga", DisplayName = "'Sakamoto desu ga?' chapters")]
        [DataRow("http://www.mangapanda.com/akame-ga-kiru", DisplayName = "'Akame ga Kill!' chapters")]
        [DataRow("http://www.mangapanda.com/nisekoi", DisplayName = "'Nisekoi' chapters")]
        public override async Task GetChaptersAsync_ByUri_ShouldReturn_CorrectData(string url)
        {
            await base.GetChaptersAsync_ByUri_ShouldReturn_CorrectData(url);
        }

        [DataTestMethod]
        [TestMethodCategory("GetChapters", "Async")]
        [TestMethodDelay(5000)]
        [DataRow("http://www.mangapanda.com/sakamoto-desu-ga", DisplayName = "'Sakamoto desu ga?' chapters")]
        [DataRow("http://www.mangapanda.com/akame-ga-kiru", DisplayName = "'Akame ga Kill!' chapters")]
        [DataRow("http://www.mangapanda.com/nisekoi", DisplayName = "'Nisekoi' chapters")]
        public override async Task GetChaptersAsync_ByMangaObject_ShouldReturn_CorrectData(string url)
        {
            await base.GetChaptersAsync_ByMangaObject_ShouldReturn_CorrectData(url);
        }

        #endregion Async

        #endregion GetChapters

        #region GetPages

        #region Sync

        [DataTestMethod]
        [TestMethodCategory("GetPages")]
        [TestMethodDelay(5000)]
        [DataRow("http://www.mangapanda.com/sakamoto-desu-ga/1", DisplayName = "'Sakamoto desu ga?' ch1 pages")]
        [DataRow("http://www.mangapanda.com/sakamoto-desu-ga/16", DisplayName = "'Sakamoto desu ga?' ch16 pages")]
        [DataRow("http://www.mangapanda.com/akame-ga-kiru/1", DisplayName = "'Akame ga Kill!' ch1 pages")]
        [DataRow("http://www.mangapanda.com/akame-ga-kiru/7", DisplayName = "'Akame ga Kill!' ch7 pages")]
        [DataRow("http://www.mangapanda.com/nisekoi/1", DisplayName = "'Nisekoi' ch1 pages")]
        [DataRow("http://www.mangapanda.com/nisekoi/44", DisplayName = "'Nisekoi' ch44 pages")]
        public override void GetPages_ByUrlString_ShouldReturn_CorrectData(string url)
        {
            base.GetPages_ByUrlString_ShouldReturn_CorrectData(url);
        }

        [DataTestMethod]
        [TestMethodCategory("GetPages")]
        [TestMethodDelay(5000)]
        [DataRow("http://www.mangapanda.com/sakamoto-desu-ga/1", DisplayName = "'Sakamoto desu ga?' ch1 pages")]
        [DataRow("http://www.mangapanda.com/sakamoto-desu-ga/16", DisplayName = "'Sakamoto desu ga?' ch16 pages")]
        [DataRow("http://www.mangapanda.com/akame-ga-kiru/1", DisplayName = "'Akame ga Kill!' ch1 pages")]
        [DataRow("http://www.mangapanda.com/akame-ga-kiru/7", DisplayName = "'Akame ga Kill!' ch7 pages")]
        [DataRow("http://www.mangapanda.com/nisekoi/1", DisplayName = "'Nisekoi' ch1 pages")]
        [DataRow("http://www.mangapanda.com/nisekoi/44", DisplayName = "'Nisekoi' ch44 pages")]
        public override void GetPages_ByUri_ShouldReturn_CorrectData(string url)
        {
            base.GetPages_ByUri_ShouldReturn_CorrectData(url);
        }

        [DataTestMethod]
        [TestMethodCategory("GetPages")]
        [TestMethodDelay(5000)]
        [DataRow("http://www.mangapanda.com/sakamoto-desu-ga/1", DisplayName = "'Sakamoto desu ga?' ch1 pages")]
        [DataRow("http://www.mangapanda.com/sakamoto-desu-ga/16", DisplayName = "'Sakamoto desu ga?' ch16 pages")]
        [DataRow("http://www.mangapanda.com/akame-ga-kiru/1", DisplayName = "'Akame ga Kill!' ch1 pages")]
        [DataRow("http://www.mangapanda.com/akame-ga-kiru/7", DisplayName = "'Akame ga Kill!' ch7 pages")]
        [DataRow("http://www.mangapanda.com/nisekoi/1", DisplayName = "'Nisekoi' ch1 pages")]
        [DataRow("http://www.mangapanda.com/nisekoi/44", DisplayName = "'Nisekoi' ch44 pages")]
        public override void GetPages_ByChapter_ShouldReturn_CorrectData(string url)
        {
            base.GetPages_ByChapter_ShouldReturn_CorrectData(url);
        }

        #endregion Sync

        #region Async

        [DataTestMethod]
        [TestMethodCategory("GetPages", "Async")]
        [TestMethodDelay(5000)]
        [DataRow("http://www.mangapanda.com/sakamoto-desu-ga/1", DisplayName = "'Sakamoto desu ga?' ch1 pages")]
        [DataRow("http://www.mangapanda.com/sakamoto-desu-ga/16", DisplayName = "'Sakamoto desu ga?' ch16 pages")]
        [DataRow("http://www.mangapanda.com/akame-ga-kiru/1", DisplayName = "'Akame ga Kill!' ch1 pages")]
        [DataRow("http://www.mangapanda.com/akame-ga-kiru/7", DisplayName = "'Akame ga Kill!' ch7 pages")]
        [DataRow("http://www.mangapanda.com/nisekoi/1", DisplayName = "'Nisekoi' ch1 pages")]
        [DataRow("http://www.mangapanda.com/nisekoi/44", DisplayName = "'Nisekoi' ch44 pages")]
        public override async Task GetPagesAsync_ByUrlString_ShouldReturn_CorrectData(string url)
        {
            await base.GetPagesAsync_ByUrlString_ShouldReturn_CorrectData(url);
        }

        [DataTestMethod]
        [TestMethodCategory("GetPages", "Async")]
        [TestMethodDelay(5000)]
        [DataRow("http://www.mangapanda.com/sakamoto-desu-ga/1", DisplayName = "'Sakamoto desu ga?' ch1 pages")]
        [DataRow("http://www.mangapanda.com/sakamoto-desu-ga/16", DisplayName = "'Sakamoto desu ga?' ch16 pages")]
        [DataRow("http://www.mangapanda.com/akame-ga-kiru/1", DisplayName = "'Akame ga Kill!' ch1 pages")]
        [DataRow("http://www.mangapanda.com/akame-ga-kiru/7", DisplayName = "'Akame ga Kill!' ch7 pages")]
        [DataRow("http://www.mangapanda.com/nisekoi/1", DisplayName = "'Nisekoi' ch1 pages")]
        [DataRow("http://www.mangapanda.com/nisekoi/44", DisplayName = "'Nisekoi' ch44 pages")]
        public override async Task GetPagesAsync_ByUri_ShouldReturn_CorrectData(string url)
        {
            await base.GetPagesAsync_ByUri_ShouldReturn_CorrectData(url);
        }

        [DataTestMethod]
        [TestMethodCategory("GetPages", "Async")]
        [TestMethodDelay(5000)]
        [DataRow("http://www.mangapanda.com/sakamoto-desu-ga/1", DisplayName = "'Sakamoto desu ga?' ch1 pages")]
        [DataRow("http://www.mangapanda.com/sakamoto-desu-ga/16", DisplayName = "'Sakamoto desu ga?' ch16 pages")]
        [DataRow("http://www.mangapanda.com/akame-ga-kiru/1", DisplayName = "'Akame ga Kill!' ch1 pages")]
        [DataRow("http://www.mangapanda.com/akame-ga-kiru/7", DisplayName = "'Akame ga Kill!' ch7 pages")]
        [DataRow("http://www.mangapanda.com/nisekoi/1", DisplayName = "'Nisekoi' ch1 pages")]
        [DataRow("http://www.mangapanda.com/nisekoi/44", DisplayName = "'Nisekoi' ch44 pages")]
        public override async Task GetPagesAsync_ByChapter_ShouldReturn_CorrectData(string url)
        {
            await base.GetPagesAsync_ByChapter_ShouldReturn_CorrectData(url);
        }

        #endregion Async

        #endregion GetPages
    }
}