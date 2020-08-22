using MangaParser.Core.Exceptions;
using MangaParser.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MangaParser.Core.Models
{
    public abstract class Parser : IParser, IEquatable<Parser>
    {
        #region Constructors

        /// <summary>
        ///
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <exception cref="ArgumentNullException"></exception>
        protected Parser(Uri baseUrl)
        {
            if (baseUrl is null)
            {
                throw new ArgumentNullException(nameof(baseUrl));
            }

            BaseUrl = baseUrl;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <exception cref="UriFormatException"></exception>
        protected Parser(string baseUrl)
        {
            if (Uri.IsWellFormedUriString(baseUrl, UriKind.Absolute))
            {
                BaseUrl = new Uri(baseUrl);
            }
            else
            {
                throw new UriFormatException($"{nameof(baseUrl)} must contain a valid url.");
            }
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Base url of a manga site for future parsing.
        /// </summary>
        public Uri BaseUrl { get; }

        #endregion Properties

        #region Methods

        #region IParserSync

        #region Search

        /// <summary>
        ///
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public abstract IEnumerable<IMangaObject> SearchManga(string query);

        #endregion Search

        #region GetManga

        /// <param name="manga"></param>
        ///<inheritdoc cref="GetManga(Uri)"/>
        ///<exception cref="ArgumentNullException"></exception>
        public virtual IMangaObject GetManga(IMangaObject manga)
        {
            if (manga is null)
            {
                throw new ArgumentNullException(nameof(manga));
            }
            else
            {
                return GetManga(manga.Url);
            }
        }

        /// <inheritdoc cref="GetManga(Uri)"/>
        /// <exception cref="UriFormatException"></exception>
        public virtual IMangaObject GetManga(string url)
        {
            if (Uri.TryCreate(url, UriKind.Absolute, out var result))
            {
                return GetManga(result);
            }
            else
            {
                throw new UriFormatException($"{nameof(url)} is an invalid url.");
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="BaseHostNotMatchException"></exception>
        public abstract IMangaObject GetManga(Uri url);

        #endregion GetManga

        #region GetChapters

        /// <param name="manga"></param>
        /// <inheritdoc cref="GetChapters(Uri)"/>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual IEnumerable<IChapter> GetChapters(IMangaObject manga)
        {
            if (manga is null)
            {
                throw new ArgumentNullException(nameof(manga));
            }
            else
            {
                return GetChapters(manga.Url);
            }
        }

        /// <inheritdoc cref="GetChapters(Uri)"/>
        /// <exception cref="UriFormatException"></exception>
        public virtual IEnumerable<IChapter> GetChapters(string url)
        {
            if (Uri.TryCreate(url, UriKind.Absolute, out var result))
            {
                return GetChapters(result);
            }
            else
            {
                throw new UriFormatException($"{nameof(url)} is an invalid url.");
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="BaseHostNotMatchException"></exception>
        public abstract IEnumerable<IChapter> GetChapters(Uri url);

        #endregion GetChapters

        #region GetPages

        /// <param name="chapter"></param>
        /// <inheritdoc cref="GetPages(Uri)"/>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual IEnumerable<IDataBase> GetPages(IChapter chapter)
        {
            if (chapter is null)
            {
                throw new ArgumentNullException(nameof(chapter));
            }
            else
            {
                return GetPages(chapter.Url);
            }
        }

        /// <inheritdoc cref="GetPages(Uri)"/>
        /// <exception cref="UriFormatException"></exception>
        public virtual IEnumerable<IDataBase> GetPages(string url)
        {
            if (Uri.TryCreate(url, UriKind.Absolute, out var result))
            {
                return GetPages(result);
            }
            else
            {
                throw new UriFormatException($"{nameof(url)} is an invalid url.");
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="BaseHostNotMatchException"></exception>
        public abstract IEnumerable<IDataBase> GetPages(Uri url);

        #endregion GetPages

        #endregion IParserSync

        #region IParserAsync

        #region Search

        /// <inheritdoc cref="SearchManga(string)"/>
        public abstract Task<IEnumerable<IMangaObject>> SearchMangaAsync(string query);

        #endregion Search

        #region GetManga

        ///<inheritdoc cref="GetManga(IMangaObject)"/>
        public virtual Task<IMangaObject> GetMangaAsync(IMangaObject mangaThumb)
        {
            if (mangaThumb is null)
            {
                throw new ArgumentNullException(nameof(mangaThumb));
            }
            else
            {
                return GetMangaAsync(mangaThumb.Url);
            }
        }

        ///<inheritdoc cref="GetManga(string)"/>
        public virtual Task<IMangaObject> GetMangaAsync(string url)
        {
            if (Uri.TryCreate(url, UriKind.Absolute, out var result))
            {
                return GetMangaAsync(result);
            }
            else
            {
                throw new UriFormatException($"{nameof(url)} is an invalid url.");
            }
        }

        ///<inheritdoc cref="GetManga(Uri)"/>
        public abstract Task<IMangaObject> GetMangaAsync(Uri url);

        #endregion GetManga

        #region GetChapters

        ///<inheritdoc cref="GetChapters(IMangaObject)"/>
        public virtual Task<IEnumerable<IChapter>> GetChaptersAsync(IMangaObject manga)
        {
            if (manga is null)
            {
                throw new ArgumentNullException(nameof(manga));
            }
            else
            {
                return GetChaptersAsync(manga.Url);
            }
        }

        ///<inheritdoc cref="GetChapters(string)"/>
        public virtual Task<IEnumerable<IChapter>> GetChaptersAsync(string url)
        {
            if (Uri.TryCreate(url, UriKind.Absolute, out var result))
            {
                return GetChaptersAsync(result);
            }
            else
            {
                throw new UriFormatException($"{nameof(url)} is an invalid url.");
            }
        }

        ///<inheritdoc cref="GetChapters(Uri)"/>
        public abstract Task<IEnumerable<IChapter>> GetChaptersAsync(Uri url);

        #endregion GetChapters

        #region GetPages

        ///<inheritdoc cref="GetPages(IChapter)"/>
        public virtual Task<IEnumerable<IDataBase>> GetPagesAsync(IChapter chapter)
        {
            if (chapter is null)
            {
                throw new ArgumentNullException(nameof(chapter));
            }
            else
            {
                return GetPagesAsync(chapter.Url);
            }
        }

        ///<inheritdoc cref="GetPages(string)"/>
        public virtual Task<IEnumerable<IDataBase>> GetPagesAsync(string url)
        {
            if (Uri.TryCreate(url, UriKind.Absolute, out var result))
            {
                return GetPagesAsync(result);
            }
            else
            {
                throw new UriFormatException($"{nameof(url)} is an invalid url.");
            }
        }

        ///<inheritdoc cref="GetPages(Uri)"/>
        public abstract Task<IEnumerable<IDataBase>> GetPagesAsync(Uri url);

        #endregion GetPages

        #endregion IParserAsync

        /// <inheritdoc cref="System.Net.WebUtility.HtmlDecode(string)"/>
        /// <param name="htmlText">The string to decode.</param>
        protected virtual string Decode(string htmlText)
        {
            if (!String.IsNullOrWhiteSpace(htmlText))
            {
                string text = System.Net.WebUtility.HtmlDecode(htmlText).Trim();

                // Remove all whitespaces
                return String.Join(" ", text.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
            }
            else
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// Returns a full url with a <see cref="BaseUrl"/> part.
        /// </summary>
        /// <example>Call this method if you want to get a full url if you only have a relative path, for example from a href attribute:
        /// <code>
        /// Uri fullUrl = GetFullUrl("/list/genre/comedy");
        /// </code>
        /// </example>
        /// <param name="path">A relative path from a href attribute.</param>
        /// <returns>A full url with a <see cref="BaseUrl"/> part.</returns>
        protected virtual Uri GetFullUrl(string path)
        {
            UriBuilder uriBuilder = new UriBuilder(BaseUrl.Scheme, BaseUrl.Host, -1, path);
            return uriBuilder.Uri;
        }

        /// <inheritdoc cref="GetFullUrl(string)"/>
        /// <param name="query">A query string. Should begins with a question mark (?).</param>
        protected virtual Uri GetFullUrl(string path, string query)
        {
            UriBuilder uriBuilder = new UriBuilder(BaseUrl.Scheme, BaseUrl.Host, -1, path, query);
            return uriBuilder.Uri;
        }

        #region Default overrides

        public override bool Equals(object obj)
        {
            return Equals(obj as Parser);
        }

        public bool Equals(Parser other)
        {
            return other != null &&
                   EqualityComparer<Uri>.Default.Equals(BaseUrl, other.BaseUrl);
        }

        public override int GetHashCode()
        {
            return -1440343791 + EqualityComparer<Uri>.Default.GetHashCode(BaseUrl);
        }

        public override string ToString()
        {
            return BaseUrl.AbsoluteUri;
        }

        #endregion Default overrides

        #endregion Methods
    }
}