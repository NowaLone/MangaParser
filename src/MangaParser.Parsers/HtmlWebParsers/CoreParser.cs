using HtmlAgilityPack;
using MangaParser.Core.Exceptions;
using MangaParser.Core.Interfaces;
using MangaParser.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MangaParser.Parsers.HtmlWebParsers
{
    public abstract class CoreParser : Parser
    {
        #region Constructors

        ///<inheritdoc cref="Parser(Uri)"/>
        protected CoreParser(Uri baseUrl) : base(baseUrl)
        {
            Web = new HtmlWeb();
        }

        ///<inheritdoc cref="Parser(string)"/>
        protected CoreParser(string baseUrl) : base(baseUrl)
        {
            Web = new HtmlWeb();
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
        public override IEnumerable<IMangaObject> SearchManga(string query)
        {
            query = query is null ? String.Empty : query.Replace(' ', '+');

            var htmlDoc = Web.Load(BaseUrl + $"/search/advanced?q={query}", "POST");

            return SearchMangaCore(htmlDoc);
        }

        #endregion Search

        #region GetManga

        public override IMangaObject GetManga(Uri url)
        {
            if (url is null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            if (BaseUrl.Host != url.Host)
            {
                throw new BaseHostNotMatchException(BaseUrl.Host, url.Host, nameof(url));
            }

            var htmlDoc = Web.Load(url);

            return GetMangaCore(htmlDoc, url);
        }

        #endregion GetManga

        #region GetChapters

        public override IEnumerable<IChapter> GetChapters(Uri url)
        {
            if (url is null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            if (BaseUrl.Host != url.Host)
            {
                throw new BaseHostNotMatchException(BaseUrl.Host, url.Host, nameof(url));
            }

            var htmlDoc = Web.Load(url);

            return GetChaptersCore(htmlDoc, url);
        }

        #endregion GetChapters

        #region GetPages

        public override IEnumerable<IDataBase> GetPages(Uri url)
        {
            if (url is null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            if (BaseUrl.Host != url.Host)
            {
                throw new BaseHostNotMatchException(BaseUrl.Host, url.Host, nameof(url));
            }

            var htmlDoc = Web.Load(url);

            return GetPagesCore(htmlDoc, url);
        }

        #endregion GetPages

        #endregion IParserSync

        #region IParserAsync

        #region Search

        public override Task<IEnumerable<IMangaObject>> SearchMangaAsync(string query)
        {
            // HtmlAgilityPack does't support async load with custom method (?)
            return Task.Run(() => SearchManga(query));
        }

        #endregion Search

        #region GetManga

        public override async Task<IMangaObject> GetMangaAsync(Uri url)
        {
            if (url is null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            if (BaseUrl.Host != url.Host)
            {
                throw new BaseHostNotMatchException(BaseUrl.Host, url.Host, nameof(url));
            }

            var htmlDoc = await Web.LoadFromWebAsync(url.OriginalString).ConfigureAwait(false);

            return GetMangaCore(htmlDoc, url);
        }

        #endregion GetManga

        #region GetChapters

        public override async Task<IEnumerable<IChapter>> GetChaptersAsync(Uri url)
        {
            if (url is null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            if (BaseUrl.Host != url.Host)
            {
                throw new BaseHostNotMatchException(BaseUrl.Host, url.Host, nameof(url));
            }

            var htmlDoc = await Web.LoadFromWebAsync(url.OriginalString).ConfigureAwait(false);

            return GetChaptersCore(htmlDoc, url);
        }

        #endregion GetChapters

        #region GetPages

        public override async Task<IEnumerable<IDataBase>> GetPagesAsync(Uri url)
        {
            if (url is null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            if (BaseUrl.Host != url.Host)
            {
                throw new BaseHostNotMatchException(BaseUrl.Host, url.Host, nameof(url));
            }

            var htmlDoc = await Web.LoadFromWebAsync(url.OriginalString).ConfigureAwait(false);

            return GetPagesCore(htmlDoc, url);
        }

        #endregion GetPages

        #endregion IParserAsync

        #region Abstract Methods

        protected abstract IEnumerable<IMangaObject> SearchMangaCore(HtmlDocument htmlDoc);

        protected abstract IMangaObject GetMangaCore(HtmlDocument htmlDoc, Uri url);

        protected abstract IEnumerable<IChapter> GetChaptersCore(HtmlDocument htmlDoc, Uri url);

        protected abstract IEnumerable<IDataBase> GetPagesCore(HtmlDocument htmlDoc, Uri url);

        #endregion Abstract Methods

        #endregion Methods
    }
}