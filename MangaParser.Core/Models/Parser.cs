using MangaParser.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MangaParser.Core.Models
{
    public abstract class Parser : IParser
    {
        #region Constructors

        protected Parser(Uri baseUri)
        {
            BaseUri = baseUri;
        }

        protected Parser(string baseUri)
        {
            if (Uri.IsWellFormedUriString(baseUri, UriKind.Absolute))
                BaseUri = new Uri(baseUri);
        }

        #endregion Constructors

        #region Properties

        public Uri BaseUri { get; }

        #endregion Properties

        #region Methods

        #region Synchronous Methods

        public virtual IEnumerable<IChapter> GetChapters(IMangaThumb manga)
        {
            if (manga != null)
                return GetChapters(manga.MangaUri.OriginalString);
            else
                throw new ArgumentNullException(nameof(manga));
        }

        public abstract IEnumerable<IChapter> GetChapters(string mangaUri);

        public virtual IEnumerable<IChapter> GetChapters(Uri mangaUri)
        {
            if (mangaUri != null)
                return GetChapters(mangaUri.OriginalString);
            else
                throw new ArgumentNullException(nameof(mangaUri));
        }

        public virtual IManga GetManga(IMangaThumb mangaThumb)
        {
            if (mangaThumb != null)
                return GetManga(mangaThumb.MangaUri.OriginalString);
            else
                throw new ArgumentNullException(nameof(mangaThumb));
        }

        public abstract IManga GetManga(string mangaUri);

        public virtual IManga GetManga(Uri mangaUri)
        {
            if (mangaUri != null)
                return GetManga(mangaUri.OriginalString);
            else
                throw new ArgumentNullException(nameof(mangaUri));
        }

        public virtual IEnumerable<IPage> GetPages(IChapter chapter)
        {
            if (chapter != null)
                foreach (var item in GetPages(chapter.ChapterUri.OriginalString))
                    yield return item;
            else
                throw new ArgumentNullException(nameof(chapter));
        }

        public abstract IEnumerable<IPage> GetPages(string chapterUri);

        public virtual IEnumerable<IPage> GetPages(Uri chapterUri)
        {
            if (chapterUri != null)
                foreach (var item in GetPages(chapterUri.OriginalString))
                    yield return item;
            else
                throw new ArgumentNullException(nameof(chapterUri));
        }

        public abstract IEnumerable<IMangaThumb> SearchManga(string query);

        #endregion Synchronous Methods

        #region Asynchronous Methods

        public virtual async Task<IEnumerable<IChapter>> GetChaptersAsync(IMangaThumb manga)
        {
            if (manga != null)
                return await GetChaptersAsync(manga.MangaUri.OriginalString);
            else
                throw new ArgumentNullException(nameof(manga));
        }

        public abstract Task<IEnumerable<IChapter>> GetChaptersAsync(string mangaUri);

        public virtual async Task<IEnumerable<IChapter>> GetChaptersAsync(Uri mangaUri)
        {
            if (mangaUri != null)
                return await GetChaptersAsync(mangaUri.OriginalString);
            else
                throw new ArgumentNullException(nameof(mangaUri));
        }

        public virtual async Task<IManga> GetMangaAsync(IMangaThumb mangaThumb)
        {
            if (mangaThumb != null)
                return await GetMangaAsync(mangaThumb.MangaUri.OriginalString);
            else
                throw new ArgumentNullException(nameof(mangaThumb));
        }

        public abstract Task<IManga> GetMangaAsync(string mangaUri);

        public virtual async Task<IManga> GetMangaAsync(Uri mangaUri)
        {
            if (mangaUri != null)
                return await GetMangaAsync(mangaUri.OriginalString);
            else
                throw new ArgumentNullException(nameof(mangaUri));
        }

        public virtual async Task<IEnumerable<IPage>> GetPagesAsync(IChapter chapter)
        {
            if (chapter != null)
                return await GetPagesAsync(chapter.ChapterUri.OriginalString);
            else
                throw new ArgumentNullException(nameof(chapter));
        }

        public abstract Task<IEnumerable<IPage>> GetPagesAsync(string chapterUri);

        public virtual async Task<IEnumerable<IPage>> GetPagesAsync(Uri chapterUri)
        {
            if (chapterUri != null)
                return await GetPagesAsync(chapterUri.OriginalString);
            else
                throw new ArgumentNullException(nameof(chapterUri));
        }

        public abstract Task<IEnumerable<IMangaThumb>> SearchMangaAsync(string query);

        #endregion Asynchronous Methods

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

        #endregion Methods
    }
}