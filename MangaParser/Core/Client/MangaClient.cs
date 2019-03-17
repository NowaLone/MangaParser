using MangaParser.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace MangaParser.Core.Client
{
    public class MangaClient
    {
        private readonly List<IParser> parsersList;

        public MangaClient()
        {
            parsersList = new List<IParser>();
        }

        private IParser GetParser(string uri)
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

        public void AddParser(IParser parser)
        {
            parsersList.Add(parser);
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

        public IManga GetManga(string mangaUri)
        {
            return GetParser(mangaUri)?.GetManga(mangaUri);
        }
        public IManga GetManga(Uri mangaUri)
        {
            return GetParser(mangaUri.OriginalString)?.GetManga(mangaUri);
        }
        public IManga GetManga(IMangaThumb mangaThumb)
        {
            return GetParser(mangaThumb.MangaUri.OriginalString)?.GetManga(mangaThumb);
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
        public IEnumerable<IChapter> GetChapters(IMangaThumb manga)
        {
            foreach (IChapter chapter in GetParser(manga.MangaUri.OriginalString)?.GetChapters(manga))
            {
                yield return chapter;
            }
        }

        public IEnumerable<IPage> GetChapterPages(string chapterUri)
        {
            foreach (IPage page in GetParser(chapterUri)?.GetPages(chapterUri))
            {
                yield return page;
            }
        }
        public IEnumerable<IPage> GetChapterPages(Uri chapterUri)
        {
            foreach (IPage page in GetParser(chapterUri.OriginalString)?.GetPages(chapterUri))
            {
                yield return page;
            }
        }
        public IEnumerable<IPage> GetChapterPages(IChapter chapter)
        {
            foreach (IPage page in GetParser(chapter.ChapterUri.OriginalString)?.GetPages(chapter))
            {
                yield return page;
            }
        }
    }
}