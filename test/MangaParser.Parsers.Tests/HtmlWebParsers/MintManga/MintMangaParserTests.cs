using MangaParser.Core.Interfaces;
using MangaParser.Parsers.HtmlWebParsers.MintManga;
using MangaParser.Parsers.Tests.Attributes;
using MangaParser.Parsers.Tests.HtmlWebParsers.ReadManga;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace MangaParser.Parsers.Tests.HtmlWebParsers.MintManga
{
    [TestClass]
    [TestClassCategory("Parser", "MintManga")]
    public class MintMangaParserTests : ReadMangaParserTests
    {
        #region Properties

        protected override string ParserPath => "MintManga";
        protected override IParser GetNewParser => new MintMangaParser();

        #endregion Properties

        #region Search

        #region Sync

        [DataTestMethod]
        [TestMethodCategory("Search")]
        [TestMethodDelay()]
        [DataRow("vinland", DisplayName = "'vinland' query")]
        [DataRow("akame", DisplayName = "'akame' query")]
        [DataRow("to Love-Ru", DisplayName = "Multiple word query")]
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
        [TestMethodDelay()]
        [DataRow("vinland", DisplayName = "'vinland' query")]
        [DataRow("akame", DisplayName = "'akame' query")]
        [DataRow("to Love-Ru", DisplayName = "Multiple word query")]
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
        [TestMethodDelay()]
        [DataRow("https://mintmanga.live/saga_o_vinlande", DisplayName = "'Vinland Saga' url")]
        [DataRow("https://mintmanga.live/akame_ga_kill", DisplayName = "'Akame ga KILL!' url")]
        [DataRow("https://mintmanga.live/to_love_ru", DisplayName = "'To Love-Ru' url")]
        [DataRow("https://mintmanga.live/neobiatnyi_okean", DisplayName = "'Grand Blue' url")]
        public override void GetManga_ByUrlString_ShouldReturn_CorrectData(string url)
        {
            base.GetManga_ByUrlString_ShouldReturn_CorrectData(url);
        }

        [DataTestMethod]
        [TestMethodCategory("GetManga")]
        [TestMethodDelay()]
        [DataRow("https://mintmanga.live/saga_o_vinlande", DisplayName = "'Vinland Saga' url")]
        [DataRow("https://mintmanga.live/akame_ga_kill", DisplayName = "'Akame ga KILL!' url")]
        [DataRow("https://mintmanga.live/to_love_ru", DisplayName = "'To Love-Ru' url")]
        [DataRow("https://mintmanga.live/neobiatnyi_okean", DisplayName = "'Grand Blue' url")]
        public override void GetManga_ByUri_ShouldReturn_CorrectData(string url)
        {
            base.GetManga_ByUri_ShouldReturn_CorrectData(url);
        }

        [DataTestMethod]
        [TestMethodCategory("GetManga")]
        [TestMethodDelay()]
        [DataRow("https://mintmanga.live/saga_o_vinlande", DisplayName = "'Vinland Saga' url")]
        [DataRow("https://mintmanga.live/akame_ga_kill", DisplayName = "'Akame ga KILL!' url")]
        [DataRow("https://mintmanga.live/to_love_ru", DisplayName = "'To Love-Ru' url")]
        [DataRow("https://mintmanga.live/neobiatnyi_okean", DisplayName = "'Grand Blue' url")]
        public override void GetManga_ByMangaObject_ShouldReturn_CorrectData(string url)
        {
            base.GetManga_ByMangaObject_ShouldReturn_CorrectData(url);
        }

        #endregion Sync

        #region Async

        [DataTestMethod]
        [TestMethodCategory("GetManga", "Async")]
        [TestMethodDelay()]
        [DataRow("https://mintmanga.live/saga_o_vinlande", DisplayName = "'Vinland Saga' url")]
        [DataRow("https://mintmanga.live/akame_ga_kill", DisplayName = "'Akame ga KILL!' url")]
        [DataRow("https://mintmanga.live/to_love_ru", DisplayName = "'To Love-Ru' url")]
        [DataRow("https://mintmanga.live/neobiatnyi_okean", DisplayName = "'Grand Blue' url")]
        public override async Task GetMangaAsync_ByUrlString_ShouldReturn_CorrectData(string url)
        {
            await base.GetMangaAsync_ByUrlString_ShouldReturn_CorrectData(url);
        }

        [DataTestMethod]
        [TestMethodCategory("GetManga", "Async")]
        [TestMethodDelay()]
        [DataRow("https://mintmanga.live/saga_o_vinlande", DisplayName = "'Vinland Saga' url")]
        [DataRow("https://mintmanga.live/akame_ga_kill", DisplayName = "'Akame ga KILL!' url")]
        [DataRow("https://mintmanga.live/to_love_ru", DisplayName = "'To Love-Ru' url")]
        [DataRow("https://mintmanga.live/neobiatnyi_okean", DisplayName = "'Grand Blue' url")]
        public override async Task GetMangaAsync_ByUri_ShouldReturn_CorrectData(string url)
        {
            await base.GetMangaAsync_ByUri_ShouldReturn_CorrectData(url);
        }

        [DataTestMethod]
        [TestMethodCategory("GetManga", "Async")]
        [TestMethodDelay()]
        [DataRow("https://mintmanga.live/saga_o_vinlande", DisplayName = "'Vinland Saga' url")]
        [DataRow("https://mintmanga.live/akame_ga_kill", DisplayName = "'Akame ga KILL!' url")]
        [DataRow("https://mintmanga.live/to_love_ru", DisplayName = "'To Love-Ru' url")]
        [DataRow("https://mintmanga.live/neobiatnyi_okean", DisplayName = "'Grand Blue' url")]
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
        [TestMethodDelay()]
        [DataRow("https://mintmanga.live/saga_o_vinlande", DisplayName = "'Vinland Saga' chapters")]
        [DataRow("https://mintmanga.live/akame_ga_kill", DisplayName = "'Akame ga KILL!' chapters")]
        [DataRow("https://mintmanga.live/to_love_ru", DisplayName = "'To Love-Ru' chapters")]
        public override void GetChapters_ByUrlString_ShouldReturn_CorrectData(string url)
        {
            base.GetChapters_ByUrlString_ShouldReturn_CorrectData(url);
        }

        [DataTestMethod]
        [TestMethodCategory("GetChapters")]
        [TestMethodDelay()]
        [DataRow("https://mintmanga.live/saga_o_vinlande", DisplayName = "'Vinland Saga' chapters")]
        [DataRow("https://mintmanga.live/akame_ga_kill", DisplayName = "'Akame ga KILL!' chapters")]
        [DataRow("https://mintmanga.live/to_love_ru", DisplayName = "'To Love-Ru' chapters")]
        public override void GetChapters_ByUri_ShouldReturn_CorrectData(string url)
        {
            base.GetChapters_ByUri_ShouldReturn_CorrectData(url);
        }

        [DataTestMethod]
        [TestMethodCategory("GetChapters")]
        [TestMethodDelay()]
        [DataRow("https://mintmanga.live/saga_o_vinlande", DisplayName = "'Vinland Saga' chapters")]
        [DataRow("https://mintmanga.live/akame_ga_kill", DisplayName = "'Akame ga KILL!' chapters")]
        [DataRow("https://mintmanga.live/to_love_ru", DisplayName = "'To Love-Ru' chapters")]
        public override void GetChapters_ByMangaObject_ShouldReturn_CorrectData(string url)
        {
            base.GetChapters_ByMangaObject_ShouldReturn_CorrectData(url);
        }

        #endregion Sync

        #region Async

        [DataTestMethod]
        [TestMethodCategory("GetChapters", "Async")]
        [TestMethodDelay()]
        [DataRow("https://mintmanga.live/saga_o_vinlande", DisplayName = "'Vinland Saga' chapters")]
        [DataRow("https://mintmanga.live/akame_ga_kill", DisplayName = "'Akame ga KILL!' chapters")]
        [DataRow("https://mintmanga.live/to_love_ru", DisplayName = "'To Love-Ru' chapters")]
        public override async Task GetChaptersAsync_ByUrlString_ShouldReturn_CorrectData(string url)
        {
            await base.GetChaptersAsync_ByUrlString_ShouldReturn_CorrectData(url);
        }

        [DataTestMethod]
        [TestMethodCategory("GetChapters", "Async")]
        [TestMethodDelay()]
        [DataRow("https://mintmanga.live/saga_o_vinlande", DisplayName = "'Vinland Saga' chapters")]
        [DataRow("https://mintmanga.live/akame_ga_kill", DisplayName = "'Akame ga KILL!' chapters")]
        [DataRow("https://mintmanga.live/to_love_ru", DisplayName = "'To Love-Ru' chapters")]
        public override async Task GetChaptersAsync_ByUri_ShouldReturn_CorrectData(string url)
        {
            await base.GetChaptersAsync_ByUri_ShouldReturn_CorrectData(url);
        }

        [DataTestMethod]
        [TestMethodCategory("GetChapters", "Async")]
        [TestMethodDelay()]
        [DataRow("https://mintmanga.live/saga_o_vinlande", DisplayName = "'Vinland Saga' chapters")]
        [DataRow("https://mintmanga.live/akame_ga_kill", DisplayName = "'Akame ga KILL!' chapters")]
        [DataRow("https://mintmanga.live/to_love_ru", DisplayName = "'To Love-Ru' chapters")]
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
        [TestMethodDelay()]
        [DataRow("https://mintmanga.live/saga_o_vinlande/vol1/1?mtr=1", DisplayName = "'Vinland Saga' ch1 pages")]
        [DataRow("https://mintmanga.live/saga_o_vinlande/vol22/154?mtr=1", DisplayName = "'Vinland Saga' ch154 pages")]
        [DataRow("https://mintmanga.live/akame_ga_kill/vol1/1?mtr=1", DisplayName = "'Akame ga KILL!' ch1 pages")]
        [DataRow("https://mintmanga.live/akame_ga_kill/vol9/44?mtr=1", DisplayName = "'Akame ga KILL!' ch44 pages")]
        [DataRow("https://mintmanga.live/to_love_ru/vol1/1?mtr=1", DisplayName = "'To Love-Ru' ch1 pages")]
        [DataRow("https://mintmanga.live/to_love_ru/vol6/52?mtr=1", DisplayName = "'To Love-Ru' ch3 pages")]
        public override void GetPages_ByUrlString_ShouldReturn_CorrectData(string url)
        {
            base.GetPages_ByUrlString_ShouldReturn_CorrectData(url);
        }

        [DataTestMethod]
        [TestMethodCategory("GetPages")]
        [TestMethodDelay()]
        [DataRow("https://mintmanga.live/saga_o_vinlande/vol1/1?mtr=1", DisplayName = "'Vinland Saga' ch1 pages")]
        [DataRow("https://mintmanga.live/saga_o_vinlande/vol22/154?mtr=1", DisplayName = "'Vinland Saga' ch16 pages")]
        [DataRow("https://mintmanga.live/akame_ga_kill/vol1/1?mtr=1", DisplayName = "'Akame ga KILL!' ch1 pages")]
        [DataRow("https://mintmanga.live/akame_ga_kill/vol9/44?mtr=1", DisplayName = "'Akame ga KILL!' ch7 pages")]
        [DataRow("https://mintmanga.live/to_love_ru/vol1/1?mtr=1", DisplayName = "'To Love-Ru' ch1 pages")]
        [DataRow("https://mintmanga.live/to_love_ru/vol6/52?mtr=1", DisplayName = "'To Love-Ru' ch3 pages")]
        public override void GetPages_ByUri_ShouldReturn_CorrectData(string url)
        {
            base.GetPages_ByUri_ShouldReturn_CorrectData(url);
        }

        [DataTestMethod]
        [TestMethodCategory("GetPages")]
        [TestMethodDelay()]
        [DataRow("https://mintmanga.live/saga_o_vinlande/vol1/1?mtr=1", DisplayName = "'Vinland Saga' ch1 pages")]
        [DataRow("https://mintmanga.live/saga_o_vinlande/vol22/154?mtr=1", DisplayName = "'Vinland Saga' ch16 pages")]
        [DataRow("https://mintmanga.live/akame_ga_kill/vol1/1?mtr=1", DisplayName = "'Akame ga KILL!' ch1 pages")]
        [DataRow("https://mintmanga.live/akame_ga_kill/vol9/44?mtr=1", DisplayName = "'Akame ga KILL!' ch7 pages")]
        [DataRow("https://mintmanga.live/to_love_ru/vol1/1?mtr=1", DisplayName = "'To Love-Ru' ch1 pages")]
        [DataRow("https://mintmanga.live/to_love_ru/vol6/52?mtr=1", DisplayName = "'To Love-Ru' ch3 pages")]
        public override void GetPages_ByChapter_ShouldReturn_CorrectData(string url)
        {
            base.GetPages_ByChapter_ShouldReturn_CorrectData(url);
        }

        #endregion Sync

        #region Async

        [DataTestMethod]
        [TestMethodCategory("GetPages", "Async")]
        [TestMethodDelay()]
        [DataRow("https://mintmanga.live/saga_o_vinlande/vol1/1?mtr=1", DisplayName = "'Vinland Saga' ch1 pages")]
        [DataRow("https://mintmanga.live/saga_o_vinlande/vol22/154?mtr=1", DisplayName = "'Vinland Saga' ch16 pages")]
        [DataRow("https://mintmanga.live/akame_ga_kill/vol1/1?mtr=1", DisplayName = "'Akame ga KILL!' ch1 pages")]
        [DataRow("https://mintmanga.live/akame_ga_kill/vol9/44?mtr=1", DisplayName = "'Akame ga KILL!' ch7 pages")]
        [DataRow("https://mintmanga.live/to_love_ru/vol1/1?mtr=1", DisplayName = "'To Love-Ru' ch1 pages")]
        [DataRow("https://mintmanga.live/to_love_ru/vol6/52?mtr=1", DisplayName = "'To Love-Ru' ch3 pages")]
        public override async Task GetPagesAsync_ByUrlString_ShouldReturn_CorrectData(string url)
        {
            await base.GetPagesAsync_ByUrlString_ShouldReturn_CorrectData(url);
        }

        [DataTestMethod]
        [TestMethodCategory("GetPages", "Async")]
        [TestMethodDelay()]
        [DataRow("https://mintmanga.live/saga_o_vinlande/vol1/1?mtr=1", DisplayName = "'Vinland Saga' ch1 pages")]
        [DataRow("https://mintmanga.live/saga_o_vinlande/vol22/154?mtr=1", DisplayName = "'Vinland Saga' ch16 pages")]
        [DataRow("https://mintmanga.live/akame_ga_kill/vol1/1?mtr=1", DisplayName = "'Akame ga KILL!' ch1 pages")]
        [DataRow("https://mintmanga.live/akame_ga_kill/vol9/44?mtr=1", DisplayName = "'Akame ga KILL!' ch7 pages")]
        [DataRow("https://mintmanga.live/to_love_ru/vol1/1?mtr=1", DisplayName = "'To Love-Ru' ch1 pages")]
        [DataRow("https://mintmanga.live/to_love_ru/vol6/52?mtr=1", DisplayName = "'To Love-Ru' ch3 pages")]
        public override async Task GetPagesAsync_ByUri_ShouldReturn_CorrectData(string url)
        {
            await base.GetPagesAsync_ByUri_ShouldReturn_CorrectData(url);
        }

        [DataTestMethod]
        [TestMethodCategory("GetPages", "Async")]
        [TestMethodDelay()]
        [DataRow("https://mintmanga.live/saga_o_vinlande/vol1/1?mtr=1", DisplayName = "'Vinland Saga' ch1 pages")]
        [DataRow("https://mintmanga.live/saga_o_vinlande/vol22/154?mtr=1", DisplayName = "'Vinland Saga' ch16 pages")]
        [DataRow("https://mintmanga.live/akame_ga_kill/vol1/1?mtr=1", DisplayName = "'Akame ga KILL!' ch1 pages")]
        [DataRow("https://mintmanga.live/akame_ga_kill/vol9/44?mtr=1", DisplayName = "'Akame ga KILL!' ch7 pages")]
        [DataRow("https://mintmanga.live/to_love_ru/vol1/1?mtr=1", DisplayName = "'To Love-Ru' ch1 pages")]
        [DataRow("https://mintmanga.live/to_love_ru/vol6/52?mtr=1", DisplayName = "'To Love-Ru' ch3 pages")]
        public override async Task GetPagesAsync_ByChapter_ShouldReturn_CorrectData(string url)
        {
            await base.GetPagesAsync_ByChapter_ShouldReturn_CorrectData(url);
        }

        #endregion Async

        #endregion GetPages
    }
}