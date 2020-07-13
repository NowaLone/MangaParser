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

        private readonly ICollection<IParser> parsers;

        #endregion Readonly Fields

        #region Constructors

        public MangaClient() : this(new List<IParser>())
        {
        }

        public MangaClient(ICollection<IParser> parsers)
        {
            this.parsers = parsers;
        }

        #endregion Constructors

        #region Methods

        #region IClient

        #region AddParser

        public virtual void AddParser(IParser parser)
        {
            if (!parsers.Contains(parser))
            {
                parsers.Add(parser);
            }
        }

        #endregion AddParser

        #region RemoveParser

        public virtual bool RemoveParser(string url)
        {
            var parser = GetParser(url);

            return parser != null && RemoveParser(parser);
        }

        public virtual bool RemoveParser(Uri url)
        {
            var parser = GetParser(url);

            return parser != null && RemoveParser(parser);
        }

        public virtual bool RemoveParser(IParser parser)
        {
            return parsers.Remove(parser);
        }

        public virtual bool RemoveParser<T>() where T : IParser
        {
            var parser = GetParser<T>();

            return parser != null && RemoveParser(parser);
        }

        #endregion RemoveParser

        #region GetParser

        /// <summary>
        ///
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        /// <exception cref="UriFormatException"></exception>
        public virtual IParser GetParser(string url)
        {
            if (Uri.TryCreate(url, UriKind.Absolute, out var result))
            {
                return GetParser(result);
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
        public virtual IParser GetParser(Uri url)
        {
            if (url is null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            return parsers.FirstOrDefault(parser => parser.BaseUrl.Equals(url));
        }

        public virtual IParser GetParser<T>() where T : IParser
        {
            return parsers.FirstOrDefault(parser => parser.GetType() == typeof(T));
        }

        #endregion GetParser

        #endregion IClient

        #region IParserSync

        #region Search

        public virtual IEnumerable<IMangaThumb> SearchManga(string query)
        {
            foreach (var parser in parsers)
            {
                foreach (var result in parser.SearchManga(query))
                {
                    yield return result;
                }
            }
        }

        #endregion Search

        #region GetManga

        public IManga GetManga(IMangaThumb manga)
        {
            var parser = GetParser(manga.MangaUri);

            if (parser is null)
            {
                throw new ArgumentException("Can't find a suitable parser.", nameof(manga));
            }

            return parser.GetManga(manga);
        }

        public virtual IManga GetManga(string url)
        {
            var parser = GetParser(url);

            if (parser is null)
            {
                throw new ArgumentException("Can't find a suitable parser.", nameof(url));
            }

            return parser.GetManga(url);
        }

        public IManga GetManga(Uri url)
        {
            var parser = GetParser(url);

            if (parser is null)
            {
                throw new ArgumentException("Can't find a suitable parser.", nameof(url));
            }

            return parser.GetManga(url);
        }

        #endregion GetManga

        #region GetChapters

        public IEnumerable<IChapter> GetChapters(IMangaThumb manga)
        {
            var parser = GetParser(manga.MangaUri);

            if (parser is null)
            {
                throw new ArgumentException("Can't find a suitable parser.", nameof(manga));
            }

            return parser.GetChapters(manga);
        }

        public virtual IEnumerable<IChapter> GetChapters(string url)
        {
            var parser = GetParser(url);

            if (parser is null)
            {
                throw new ArgumentException("Can't find a suitable parser.", nameof(url));
            }

            return parser.GetChapters(url);
        }

        public IEnumerable<IChapter> GetChapters(Uri url)
        {
            var parser = GetParser(url);

            if (parser is null)
            {
                throw new ArgumentException("Can't find a suitable parser.", nameof(url));
            }

            return parser.GetChapters(url);
        }

        #endregion GetChapters

        #region GetPages

        public IEnumerable<IPage> GetPages(IChapter chapter)
        {
            var parser = GetParser(chapter.ChapterUri);

            if (parser is null)
            {
                throw new ArgumentException("Can't find a suitable parser.", nameof(chapter));
            }

            return parser.GetPages(chapter);
        }

        public virtual IEnumerable<IPage> GetPages(string url)
        {
            var parser = GetParser(url);

            if (parser is null)
            {
                throw new ArgumentException("Can't find a suitable parser.", nameof(url));
            }

            return parser.GetPages(url);
        }

        public IEnumerable<IPage> GetPages(Uri url)
        {
            var parser = GetParser(url);

            if (parser is null)
            {
                throw new ArgumentException("Can't find a suitable parser.", nameof(url));
            }

            return parser.GetPages(url);
        }

        #endregion GetPages

        #endregion IParserSync

        #region IParserAsync

        #region Search

        public virtual Task<IEnumerable<IMangaThumb>> SearchMangaAsync(string query)
        {
            return Task.Run(() => SearchManga(query));
        }

        #endregion Search

        #region GetManga

        public virtual Task<IManga> GetMangaAsync(IMangaThumb manga)
        {
            var parser = GetParser(manga.MangaUri);

            if (parser is null)
            {
                throw new ArgumentException("Can't find a suitable parser.", nameof(manga));
            }

            return parser.GetMangaAsync(manga);
        }

        public virtual Task<IManga> GetMangaAsync(string url)
        {
            var parser = GetParser(url);

            if (parser is null)
            {
                throw new ArgumentException("Can't find a suitable parser.", nameof(url));
            }

            return parser.GetMangaAsync(url);
        }

        public virtual Task<IManga> GetMangaAsync(Uri url)
        {
            var parser = GetParser(url);

            if (parser is null)
            {
                throw new ArgumentException("Can't find a suitable parser.", nameof(url));
            }

            return parser.GetMangaAsync(url);
        }

        #endregion GetManga

        #region GetChapters

        public virtual Task<IEnumerable<IChapter>> GetChaptersAsync(IMangaThumb manga)
        {
            var parser = GetParser(manga.MangaUri);

            if (parser is null)
            {
                throw new ArgumentException("Can't find a suitable parser.", nameof(manga));
            }

            return parser.GetChaptersAsync(manga);
        }

        public virtual Task<IEnumerable<IChapter>> GetChaptersAsync(string url)
        {
            var parser = GetParser(url);

            if (parser is null)
            {
                throw new ArgumentException("Can't find a suitable parser.", nameof(url));
            }

            return parser.GetChaptersAsync(url);
        }

        public virtual Task<IEnumerable<IChapter>> GetChaptersAsync(Uri url)
        {
            var parser = GetParser(url);

            if (parser is null)
            {
                throw new ArgumentException("Can't find a suitable parser.", nameof(url));
            }

            return parser.GetChaptersAsync(url);
        }

        #endregion GetChapters

        #region GetPages

        public virtual Task<IEnumerable<IPage>> GetPagesAsync(IChapter chapter)
        {
            var parser = GetParser(chapter.ChapterUri);

            if (parser is null)
            {
                throw new ArgumentException("Can't find a suitable parser.", nameof(chapter));
            }

            return parser.GetPagesAsync(chapter);
        }

        public virtual Task<IEnumerable<IPage>> GetPagesAsync(string url)
        {
            var parser = GetParser(url);

            if (parser is null)
            {
                throw new ArgumentException("Can't find a suitable parser.", nameof(url));
            }

            return parser.GetPagesAsync(url);
        }

        public virtual Task<IEnumerable<IPage>> GetPagesAsync(Uri url)
        {
            var parser = GetParser(url);

            if (parser is null)
            {
                throw new ArgumentException("Can't find a suitable parser.", nameof(url));
            }

            return parser.GetPagesAsync(url);
        }

        #endregion GetPages

        #endregion IParserAsync

        #endregion Methods
    }
}