using MangaParser.Core.Exceptions;
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

        protected readonly ICollection<IParser> parsers;

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

            return parsers.FirstOrDefault(parser => parser.BaseUrl.Host.Equals(url.Host));
        }

        public virtual IParser GetParser<T>() where T : IParser
        {
            return parsers.FirstOrDefault(parser => parser.GetType() == typeof(T));
        }

        #endregion GetParser

        #endregion IClient

        #region IParserSync

        #region Search

        public virtual IEnumerable<IMangaObject> SearchManga(string query)
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

        public virtual IMangaObject GetManga(IMangaObject manga)
        {
            var parser = GetParser(manga.Url);

            if (parser is null)
            {
                throw new ParserNotFoundException(manga.Url);
            }

            return parser.GetManga(manga);
        }

        public virtual IMangaObject GetManga(string url)
        {
            var parser = GetParser(url);

            if (parser is null)
            {
                throw new ParserNotFoundException(url);
            }

            return parser.GetManga(url);
        }

        public virtual IMangaObject GetManga(Uri url)
        {
            var parser = GetParser(url);

            if (parser is null)
            {
                throw new ParserNotFoundException(url);
            }

            return parser.GetManga(url);
        }

        #endregion GetManga

        #region GetChapters

        public virtual IEnumerable<IChapter> GetChapters(IMangaObject manga)
        {
            var parser = GetParser(manga.Url);

            if (parser is null)
            {
                throw new ParserNotFoundException(manga.Url);
            }

            return parser.GetChapters(manga);
        }

        public virtual IEnumerable<IChapter> GetChapters(string url)
        {
            var parser = GetParser(url);

            if (parser is null)
            {
                throw new ParserNotFoundException(url);
            }

            return parser.GetChapters(url);
        }

        public virtual IEnumerable<IChapter> GetChapters(Uri url)
        {
            var parser = GetParser(url);

            if (parser is null)
            {
                throw new ParserNotFoundException(url);
            }

            return parser.GetChapters(url);
        }

        #endregion GetChapters

        #region GetPages

        public virtual IEnumerable<IDataBase> GetPages(IChapter chapter)
        {
            var parser = GetParser(chapter.Url);

            if (parser is null)
            {
                throw new ParserNotFoundException(chapter.Url);
            }

            return parser.GetPages(chapter);
        }

        public virtual IEnumerable<IDataBase> GetPages(string url)
        {
            var parser = GetParser(url);

            if (parser is null)
            {
                throw new ParserNotFoundException(url);
            }

            return parser.GetPages(url);
        }

        public virtual IEnumerable<IDataBase> GetPages(Uri url)
        {
            var parser = GetParser(url);

            if (parser is null)
            {
                throw new ParserNotFoundException(url);
            }

            return parser.GetPages(url);
        }

        #endregion GetPages

        #endregion IParserSync

        #region IParserAsync

        #region Search

        public virtual Task<IEnumerable<IMangaObject>> SearchMangaAsync(string query)
        {
            return Task.Run(() => SearchManga(query));
        }

        #endregion Search

        #region GetManga

        public virtual Task<IMangaObject> GetMangaAsync(IMangaObject manga)
        {
            var parser = GetParser(manga.Url);

            if (parser is null)
            {
                throw new ParserNotFoundException(manga.Url);
            }

            return parser.GetMangaAsync(manga);
        }

        public virtual Task<IMangaObject> GetMangaAsync(string url)
        {
            var parser = GetParser(url);

            if (parser is null)
            {
                throw new ParserNotFoundException(url);
            }

            return parser.GetMangaAsync(url);
        }

        public virtual Task<IMangaObject> GetMangaAsync(Uri url)
        {
            var parser = GetParser(url);

            if (parser is null)
            {
                throw new ParserNotFoundException(url);
            }

            return parser.GetMangaAsync(url);
        }

        #endregion GetManga

        #region GetChapters

        public virtual Task<IEnumerable<IChapter>> GetChaptersAsync(IMangaObject manga)
        {
            var parser = GetParser(manga.Url);

            if (parser is null)
            {
                throw new ParserNotFoundException(manga.Url);
            }

            return parser.GetChaptersAsync(manga);
        }

        public virtual Task<IEnumerable<IChapter>> GetChaptersAsync(string url)
        {
            var parser = GetParser(url);

            if (parser is null)
            {
                throw new ParserNotFoundException(url);
            }

            return parser.GetChaptersAsync(url);
        }

        public virtual Task<IEnumerable<IChapter>> GetChaptersAsync(Uri url)
        {
            var parser = GetParser(url);

            if (parser is null)
            {
                throw new ParserNotFoundException(url);
            }

            return parser.GetChaptersAsync(url);
        }

        #endregion GetChapters

        #region GetPages

        public virtual Task<IEnumerable<IDataBase>> GetPagesAsync(IChapter chapter)
        {
            var parser = GetParser(chapter.Url);

            if (parser is null)
            {
                throw new ParserNotFoundException(chapter.Url);
            }

            return parser.GetPagesAsync(chapter);
        }

        public virtual Task<IEnumerable<IDataBase>> GetPagesAsync(string url)
        {
            var parser = GetParser(url);

            if (parser is null)
            {
                throw new ParserNotFoundException(url);
            }

            return parser.GetPagesAsync(url);
        }

        public virtual Task<IEnumerable<IDataBase>> GetPagesAsync(Uri url)
        {
            var parser = GetParser(url);

            if (parser is null)
            {
                throw new ParserNotFoundException(url);
            }

            return parser.GetPagesAsync(url);
        }

        #endregion GetPages

        #endregion IParserAsync

        #endregion Methods
    }
}