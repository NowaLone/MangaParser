using MangaParser.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MangaParser.Core.Client
{
    public class MangaClient
    {
        private readonly List<IParser> parsersList;

        public MangaClient()
        {
            parsersList = new List<IParser>();
        }

        public void AddParser(IParser parser)
        {
            if (parser == null)
                return;

            parsersList.Add(parser);
        }

        public void RemoveParser(IParser parser)
        {
            if (parser == null)
                return;

            parsersList.Remove(parser);
        }

        public IParser GetParser(string uri)
        {
            for (int i = 0; i < parsersList.Count; i++)
            {
                if (uri.Contains(parsersList[i].BaseUri.Host))
                {
                    return parsersList[i];
                }
            }

            return null;
        }

        public IParser GetParser<T>() where T : IParser
        {
            for (int i = 0; i < parsersList.Count; i++)
            {
                if (parsersList[i].GetType() == typeof(T))
                {
                    return parsersList[i];
                }
            }

            return null;
        }

        public IEnumerable<IParser> GetParserEnumerable()
        {
            return parsersList;
        }

        #region Synchronous Methods

        public IEnumerable<IMangaThumb> SearchManga(string query)
        {
            for (int i = 0; i < parsersList.Count; i++)
            {
                foreach (IMangaThumb manga in parsersList[i].SearchManga(query))
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

        public IEnumerable<IChapter> GetChapters(string mangaUri)
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

        public IEnumerable<IPage> GetPages(string chapterUri)
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

        public IManga GetManga(string mangaUri)
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

        public async Task<IEnumerable<IMangaThumb>> SearchMangaAsync(string query)
        {
            return await Task.Run(async () =>
            {
                List<IMangaThumb> result = new List<IMangaThumb>();

                for (int i = 0; i < parsersList.Count; i++)
                {
                    result.AddRange(await parsersList[i].SearchMangaAsync(query));
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

        public async Task<IEnumerable<IChapter>> GetChaptersAsync(string mangaUri)
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

        public async Task<IEnumerable<IPage>> GetPagesAsync(string chapterUri)
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

        public async Task<IManga> GetMangaAsync(string mangaUri)
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
    }
}