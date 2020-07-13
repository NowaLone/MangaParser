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
        public abstract IEnumerable<IMangaThumb> SearchManga(string query);

        #endregion Search

        #region GetManga

        /// <param name="manga"></param>
        ///<inheritdoc cref="GetManga(Uri)"/>
        ///<exception cref="ArgumentNullException"></exception>
        public virtual IManga GetManga(IMangaThumb manga)
        {
            if (manga is null)
            {
                throw new ArgumentNullException(nameof(manga));
            }
            else
            {
                return GetManga(manga.MangaUri);
            }
        }

        /// <inheritdoc cref="GetManga(Uri)"/>
        /// <exception cref="UriFormatException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public virtual IManga GetManga(string url)
        {
            if (Uri.TryCreate(url, UriKind.Absolute, out var result))
            {
                if (BaseUrl.Host == result.Host)
                {
                    return GetManga(result);
                }
                else
                {
                    throw new ArgumentException($"Base url host and parameter host don't match.", nameof(url));
                }
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
        public abstract IManga GetManga(Uri url);

        #endregion GetManga

        #region GetChapters

        /// <param name="manga"></param>
        /// <inheritdoc cref="GetChapters(Uri)"/>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual IEnumerable<IChapter> GetChapters(IMangaThumb manga)
        {
            if (manga is null)
            {
                throw new ArgumentNullException(nameof(manga));
            }
            else
            {
                return GetChapters(manga.MangaUri);
            }
        }

        /// <inheritdoc cref="GetChapters(Uri)"/>
        /// <exception cref="UriFormatException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public virtual IEnumerable<IChapter> GetChapters(string url)
        {
            if (Uri.TryCreate(url, UriKind.Absolute, out var result))
            {
                if (BaseUrl.Host == result.Host)
                {
                    return GetChapters(result);
                }
                else
                {
                    throw new ArgumentException($"Base url host and parameter host don't match.", nameof(url));
                }
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
        public abstract IEnumerable<IChapter> GetChapters(Uri url);

        #endregion GetChapters

        #region GetPages

        /// <param name="chapter"></param>
        /// <inheritdoc cref="GetPages(Uri)"/>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual IEnumerable<IPage> GetPages(IChapter chapter)
        {
            if (chapter is null)
            {
                throw new ArgumentNullException(nameof(chapter));
            }
            else
            {
                return GetPages(chapter.ChapterUri);
            }
        }

        /// <inheritdoc cref="GetPages(Uri)"/>
        /// <exception cref="UriFormatException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public virtual IEnumerable<IPage> GetPages(string url)
        {
            if (Uri.TryCreate(url, UriKind.Absolute, out var result))
            {
                if (BaseUrl.Host == result.Host)
                {
                    return GetPages(result);
                }
                else
                {
                    throw new ArgumentException($"Base url host and parameter host don't match.", nameof(url));
                }
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
        public abstract IEnumerable<IPage> GetPages(Uri url);

        #endregion GetPages

        #endregion IParserSync

        #region IParserAsync

        #region Search

        /// <inheritdoc cref="SearchManga(string)"/>
        public abstract Task<IEnumerable<IMangaThumb>> SearchMangaAsync(string query);

        #endregion Search

        #region GetManga

        ///<inheritdoc cref="GetManga(IMangaThumb)"/>
        public virtual Task<IManga> GetMangaAsync(IMangaThumb mangaThumb)
        {
            if (mangaThumb is null)
            {
                throw new ArgumentNullException(nameof(mangaThumb));
            }
            else
            {
                return GetMangaAsync(mangaThumb.MangaUri);
            }
        }

        ///<inheritdoc cref="GetManga(string)"/>
        public virtual Task<IManga> GetMangaAsync(string url)
        {
            if (Uri.TryCreate(url, UriKind.Absolute, out var result))
            {
                if (BaseUrl.Host == result.Host)
                {
                    return GetMangaAsync(result);
                }
                else
                {
                    throw new ArgumentException($"Base url host and parameter host don't match.", nameof(url));
                }
            }
            else
            {
                throw new UriFormatException($"{nameof(url)} is an invalid url.");
            }
        }

        ///<inheritdoc cref="GetManga(Uri)"/>
        public abstract Task<IManga> GetMangaAsync(Uri url);

        #endregion GetManga

        #region GetChapters

        ///<inheritdoc cref="GetChapters(IMangaThumb)"/>
        public virtual Task<IEnumerable<IChapter>> GetChaptersAsync(IMangaThumb manga)
        {
            if (manga is null)
            {
                throw new ArgumentNullException(nameof(manga));
            }
            else
            {
                return GetChaptersAsync(manga.MangaUri);
            }
        }

        ///<inheritdoc cref="GetChapters(string)"/>
        public virtual Task<IEnumerable<IChapter>> GetChaptersAsync(string url)
        {
            if (Uri.TryCreate(url, UriKind.Absolute, out var result))
            {
                if (BaseUrl.Host == result.Host)
                {
                    return GetChaptersAsync(result);
                }
                else
                {
                    throw new ArgumentException($"Base url host and parameter host don't match.", nameof(url));
                }
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
        public virtual Task<IEnumerable<IPage>> GetPagesAsync(IChapter chapter)
        {
            if (chapter is null)
            {
                throw new ArgumentNullException(nameof(chapter));
            }
            else
            {
                return GetPagesAsync(chapter.ChapterUri);
            }
        }

        ///<inheritdoc cref="GetPages(string)"/>
        public virtual Task<IEnumerable<IPage>> GetPagesAsync(string url)
        {
            if (Uri.TryCreate(url, UriKind.Absolute, out var result))
            {
                if (BaseUrl.Host == result.Host)
                {
                    return GetPagesAsync(result);
                }
                else
                {
                    throw new ArgumentException($"Base url host and parameter host don't match.", nameof(url));
                }
            }
            else
            {
                throw new UriFormatException($"{nameof(url)} is an invalid url.");
            }
        }

        ///<inheritdoc cref="GetPages(Uri)"/>
        public abstract Task<IEnumerable<IPage>> GetPagesAsync(Uri url);

        #endregion GetPages

        #endregion IParserAsync

        protected virtual string Decode(string htmlText)
        {
            if (!String.IsNullOrEmpty(htmlText))
            {
                var text = System.Net.WebUtility.HtmlDecode(htmlText).Trim();

                // Remove all whitespaces
                return String.Join(" ", text.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
            }
            else
                return String.Empty;
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
            return HashCode.Combine(BaseUrl);
        }

        public override string ToString()
        {
            return BaseUrl.OriginalString;
        }

        #endregion Default overrides

        #endregion Methods
    }
}