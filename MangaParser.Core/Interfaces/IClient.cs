using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MangaParser.Core.Interfaces
{
    public interface IClient
    {
        #region Methods

        IEnumerable<IMangaThumb> SearchManga(string query);

        Task<IEnumerable<IMangaThumb>> SearchMangaAsync(string query);

        IEnumerable<IChapter> GetChapters(IMangaThumb manga);

        IEnumerable<IChapter> GetChapters(string mangaUri);

        IEnumerable<IChapter> GetChapters(Uri mangaUri);

        Task<IEnumerable<IChapter>> GetChaptersAsync(IMangaThumb manga);

        Task<IEnumerable<IChapter>> GetChaptersAsync(string mangaUri);

        Task<IEnumerable<IChapter>> GetChaptersAsync(Uri mangaUri);

        IManga GetManga(IMangaThumb mangaThumb);

        IManga GetManga(string mangaUri);

        IManga GetManga(Uri mangaUri);

        Task<IManga> GetMangaAsync(IMangaThumb mangaThumb);

        Task<IManga> GetMangaAsync(string mangaUri);

        Task<IManga> GetMangaAsync(Uri mangaUri);

        IEnumerable<IPage> GetPages(IChapter chapter);

        IEnumerable<IPage> GetPages(string chapterUri);

        IEnumerable<IPage> GetPages(Uri chapterUri);

        Task<IEnumerable<IPage>> GetPagesAsync(IChapter chapter);

        Task<IEnumerable<IPage>> GetPagesAsync(string chapterUri);

        Task<IEnumerable<IPage>> GetPagesAsync(Uri chapterUri);

        #endregion Methods
    }
}