using MangaParser.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MangaParser.Core.Client
{
    public class MangaClient : IClient
    {
        #region Readonly Fields

        private readonly List<IParser> parsers;

        #endregion Readonly Fields

        #region Constructors

        public MangaClient() : this(new List<IParser>())
        {
        }

        public MangaClient(ICollection<IParser> parsers)
        {
            this.parsers = parsers.ToList();
        }

        #endregion Constructors

        #region Methods

        public void AddParser(IParser parser) => parsers.Add(parser);

        public void RemoveParser(IParser parser) => parsers.Remove(parser);

        public IParser GetParser(string uri) => parsers.FirstOrDefault(parser => uri.Contains(parser.BaseUri.Host));

        public IParser GetParser<T>() where T : IParser => parsers.FirstOrDefault(parser => parser.GetType() == typeof(T));

        #region IClient Methods

        #region Synchronous Methods

        public virtual IEnumerable<IMangaThumb> SearchManga(string query)
        {
            for (int i = 0; i < parsers.Count; i++)
            {
                foreach (IMangaThumb manga in parsers[i].SearchManga(query))
                {
                    yield return manga;
                }
            }
        }

        public IEnumerable<IChapter> GetChapters(IMangaThumb manga)
        {
            if (manga != null)
                foreach (var item in GetChapters(manga.MangaUri.OriginalString))
                    yield return item;
            else
                throw new ArgumentNullException(nameof(manga));
        }

        public virtual IEnumerable<IChapter> GetChapters(string mangaUri)
        {
            var parser = GetParser(mangaUri);

            if (parser != null)
                foreach (IChapter chapter in parser.GetChapters(mangaUri))
                {
                    yield return chapter;
                }
            else
                yield return null;
        }

        public IEnumerable<IChapter> GetChapters(Uri mangaUri)
        {
            if (mangaUri != null)
                foreach (var item in GetChapters(mangaUri.OriginalString))
                    yield return item;
            else
                throw new ArgumentNullException(nameof(mangaUri));
        }

        public IEnumerable<IPage> GetPages(IChapter chapter)
        {
            if (chapter != null)
                foreach (var item in GetPages(chapter.ChapterUri.OriginalString))
                    yield return item;
            else
                throw new ArgumentNullException(nameof(chapter));
        }

        public virtual IEnumerable<IPage> GetPages(string chapterUri)
        {
            var parser = GetParser(chapterUri);

            if (parser != null)
                foreach (IPage page in parser.GetPages(chapterUri))
                {
                    yield return page;
                }
            else
                yield return null;
        }

        public IEnumerable<IPage> GetPages(Uri chapterUri)
        {
            if (chapterUri != null)
                foreach (var item in GetPages(chapterUri.OriginalString))
                    yield return item;
            else
                throw new ArgumentNullException(nameof(chapterUri));
        }

        public IManga GetManga(IMangaThumb mangaThumb)
        {
            if (mangaThumb != null)
                return GetManga(mangaThumb.MangaUri.OriginalString);
            else
                throw new ArgumentNullException(nameof(mangaThumb));
        }

        public virtual IManga GetManga(string mangaUri)
        {
            var parser = GetParser(mangaUri);

            return parser != null ? parser.GetManga(mangaUri) : null;
        }

        public IManga GetManga(Uri mangaUri)
        {
            if (mangaUri != null)
                return GetManga(mangaUri.OriginalString);
            else
                throw new ArgumentNullException(nameof(mangaUri));
        }

        #endregion Synchronous Methods

        #region Asynchronous Methods

        public virtual async Task<IEnumerable<IMangaThumb>> SearchMangaAsync(string query)
        {
            return await Task.Run(async () =>
            {
                List<IMangaThumb> result = new List<IMangaThumb>();

                for (int i = 0; i < parsers.Count; i++)
                {
                    result.AddRange(await parsers[i].SearchMangaAsync(query));
                }

                return result;
            });
        }

        public async Task<IEnumerable<IChapter>> GetChaptersAsync(IMangaThumb manga)
        {
            if (manga != null)
                return await GetChaptersAsync(manga.MangaUri.OriginalString);
            else
                throw new ArgumentNullException(nameof(manga));
        }

        public virtual async Task<IEnumerable<IChapter>> GetChaptersAsync(string mangaUri)
        {
            var parser = GetParser(mangaUri);

            return parser != null ? await parser.GetChaptersAsync(mangaUri) : null;
        }

        public async Task<IEnumerable<IChapter>> GetChaptersAsync(Uri mangaUri)
        {
            if (mangaUri != null)
                return await GetChaptersAsync(mangaUri.OriginalString);
            else
                throw new ArgumentNullException(nameof(mangaUri));
        }

        public async Task<IEnumerable<IPage>> GetPagesAsync(IChapter chapter)
        {
            if (chapter != null)
                return await GetPagesAsync(chapter.ChapterUri.OriginalString);
            else
                throw new ArgumentNullException(nameof(chapter));
        }

        public virtual async Task<IEnumerable<IPage>> GetPagesAsync(string chapterUri)
        {
            var parser = GetParser(chapterUri);

            return parser != null ? await parser.GetPagesAsync(chapterUri) : null;
        }

        public async Task<IEnumerable<IPage>> GetPagesAsync(Uri chapterUri)
        {
            if (chapterUri != null)
                return await GetPagesAsync(chapterUri.OriginalString);
            else
                throw new ArgumentNullException(nameof(chapterUri));
        }

        public async Task<IManga> GetMangaAsync(IMangaThumb mangaThumb)
        {
            if (mangaThumb != null)
                return await GetMangaAsync(mangaThumb.MangaUri.OriginalString);
            else
                throw new ArgumentNullException(nameof(mangaThumb));
        }

        public virtual async Task<IManga> GetMangaAsync(string mangaUri)
        {
            var parser = GetParser(mangaUri);

            return parser != null ? await parser.GetMangaAsync(mangaUri) : null;
        }

        public async Task<IManga> GetMangaAsync(Uri mangaUri)
        {
            if (mangaUri != null)
                return await GetMangaAsync(mangaUri.OriginalString);
            else
                throw new ArgumentNullException(nameof(mangaUri));
        }

        #endregion Asynchronous Methods

        #endregion IClient Methods

        #endregion Methods
    }
}