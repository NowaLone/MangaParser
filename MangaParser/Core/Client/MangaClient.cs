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
            parsersList.Add(parser);
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
            foreach (IChapter chapter in GetParser(manga.MangaUri.OriginalString)?.GetChapters(manga))
            {
                yield return chapter;
            }
        }

        public IEnumerable<IChapter> GetChapters(string mangaUri)
        {
            foreach (IChapter chapter in GetParser(mangaUri)?.GetChapters(mangaUri))
            {
                yield return chapter;
            }
        }

        public IEnumerable<IChapter> GetChapters(Uri mangaUri)
        {
            foreach (IChapter chapter in GetParser(mangaUri.OriginalString)?.GetChapters(mangaUri))
            {
                yield return chapter;
            }
        }

        public IEnumerable<IPage> GetPages(IChapter chapter)
        {
            foreach (IPage page in GetParser(chapter.ChapterUri.OriginalString)?.GetPages(chapter))
            {
                yield return page;
            }
        }

        public IEnumerable<IPage> GetPages(string chapterUri)
        {
            foreach (IPage page in GetParser(chapterUri)?.GetPages(chapterUri))
            {
                yield return page;
            }
        }

        public IEnumerable<IPage> GetPages(Uri chapterUri)
        {
            foreach (IPage page in GetParser(chapterUri.OriginalString)?.GetPages(chapterUri))
            {
                yield return page;
            }
        }

        public IManga GetManga(IMangaThumb mangaThumb)
        {
            return GetParser(mangaThumb.MangaUri.OriginalString)?.GetManga(mangaThumb);
        }

        public IManga GetManga(string mangaUri)
        {
            return GetParser(mangaUri)?.GetManga(mangaUri);
        }

        public IManga GetManga(Uri mangaUri)
        {
            return GetParser(mangaUri.OriginalString)?.GetManga(mangaUri);
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
            return await GetParser(manga.MangaUri.OriginalString)?.GetChaptersAsync(manga);
        }

        public async Task<IEnumerable<IChapter>> GetChaptersAsync(string mangaUri)
        {
            return await GetParser(mangaUri)?.GetChaptersAsync(mangaUri);
        }

        public async Task<IEnumerable<IChapter>> GetChaptersAsync(Uri mangaUri)
        {
            return await GetParser(mangaUri.OriginalString)?.GetChaptersAsync(mangaUri);
        }

        public async Task<IEnumerable<IPage>> GetPagesAsync(IChapter chapter)
        {
            return await GetParser(chapter.ChapterUri.OriginalString)?.GetPagesAsync(chapter);
        }

        public async Task<IEnumerable<IPage>> GetPagesAsync(string chapterUri)
        {
            return await GetParser(chapterUri)?.GetPagesAsync(chapterUri);
        }

        public async Task<IEnumerable<IPage>> GetPagesAsync(Uri chapterUri)
        {
            return await GetParser(chapterUri.OriginalString)?.GetPagesAsync(chapterUri);
        }

        public async Task<IManga> GetMangaAsync(IMangaThumb mangaThumb)
        {
            return await GetParser(mangaThumb.MangaUri.OriginalString)?.GetMangaAsync(mangaThumb);
        }

        public async Task<IManga> GetMangaAsync(string mangaUri)
        {
            return await GetParser(mangaUri)?.GetMangaAsync(mangaUri);
        }

        public async Task<IManga> GetMangaAsync(Uri mangaUri)
        {
            return await GetParser(mangaUri.OriginalString)?.GetMangaAsync(mangaUri);
        }

        #endregion Asynchronous Methods
    }
}