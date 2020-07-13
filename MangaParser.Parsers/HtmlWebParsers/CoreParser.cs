using HtmlAgilityPack;
using MangaParser.Core.Interfaces;
using MangaParser.Core.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace MangaParser.Parsers.HtmlWebParsers
{
    public abstract class CoreParser : Parser
    {
        #region Constructors

        ///<inheritdoc cref="Parser(Uri)"/>
        protected CoreParser(Uri baseUrl) : base(baseUrl)
        {
            Web = new HtmlWeb
            {
                PreRequest = OnPreRequest
            };
        }

        ///<inheritdoc cref="Parser(string)"/>
        protected CoreParser(string baseUrl) : base(baseUrl)
        {
            Web = new HtmlWeb
            {
                PreRequest = OnPreRequest
            };
        }

        #endregion Constructors

        #region Properties

        protected virtual HtmlWeb Web { get; }

        #endregion Properties

        #region Methods

        #region IParserSync

        #region Search

        /// <summary>
        /// Abstract logic, must be overriden.
        /// </summary>
        public override IEnumerable<IMangaThumb> SearchManga(string query)
        {
            query = query is null ? String.Empty : query.Replace(' ', '+');

            var htmlDoc = Web.Load(BaseUrl + $"/search/advanced?q={query}", "POST");

            return SearchMangaCore(htmlDoc);
        }

        #endregion Search

        #region GetManga

        public override IManga GetManga(Uri url)
        {
            if (url is null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            var htmlDoc = Web.Load(url);

            return GetMangaCore(htmlDoc);
        }

        #endregion GetManga

        #region GetChapters

        public override IEnumerable<IChapter> GetChapters(Uri url)
        {
            if (url is null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            var htmlDoc = Web.Load(url);

            return GetChaptersCore(htmlDoc);
        }

        #endregion GetChapters

        #region GetPages

        public override IEnumerable<IPage> GetPages(Uri url)
        {
            if (url is null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            var htmlDoc = Web.Load(url);

            return GetPagesCore(htmlDoc);
        }

        #endregion GetPages

        #endregion IParserSync

        #region IParserAsync

        #region Search

        public override Task<IEnumerable<IMangaThumb>> SearchMangaAsync(string query)
        {
            // HtmlAgilityPack does't support async load with custom method (?)
            return Task.Run(() => SearchManga(query));
        }

        #endregion Search

        #region GetManga

        public override async Task<IManga> GetMangaAsync(Uri url)
        {
            if (url is null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            var htmlDoc = await Web.LoadFromWebAsync(url.OriginalString).ConfigureAwait(false);

            return GetMangaCore(htmlDoc);
        }

        #endregion GetManga

        #region GetChapters

        public override async Task<IEnumerable<IChapter>> GetChaptersAsync(Uri url)
        {
            if (url is null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            var htmlDoc = await Web.LoadFromWebAsync(url.OriginalString).ConfigureAwait(false);

            return GetChaptersCore(htmlDoc);
        }

        #endregion GetChapters

        #region GetPages

        public override async Task<IEnumerable<IPage>> GetPagesAsync(Uri url)
        {
            if (url is null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            var htmlDoc = await Web.LoadFromWebAsync(url.OriginalString).ConfigureAwait(false);

            return GetPagesCore(htmlDoc);
        }

        #endregion GetPages

        #endregion IParserAsync

        #region Abstract Methods

        protected abstract IEnumerable<IMangaThumb> SearchMangaCore(HtmlDocument htmlDoc);

        protected abstract IManga GetMangaCore(HtmlDocument htmlDoc);

        protected abstract IEnumerable<IChapter> GetChaptersCore(HtmlDocument htmlDoc);

        protected abstract IEnumerable<IPage> GetPagesCore(HtmlDocument htmlDoc);

        #endregion Abstract Methods

        private static bool OnPreRequest(HttpWebRequest request)
        {
            request.AllowAutoRedirect = true;
            request.MaximumAutomaticRedirections = 1;
            return true;
        }

        #endregion Methods
    }
}