using MangaParser.Core.Interfaces;

namespace MangaParser.Parsers.ReadManga
{
    public interface IReadManga : IManga
    {
        #region Properties

        IData[] Illustrators { get; }
        IData[] Magazines { get; }
        IData[] Publishers { get; }
        IData[] ReleaseDate { get; }
        IData[] Writers { get; }
        string Volumes { get; }

        #endregion Properties
    }
}