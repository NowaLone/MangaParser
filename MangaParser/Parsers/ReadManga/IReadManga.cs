using MangaParser.Core.Interfaces;

namespace MangaParser.Parsers.ReadManga
{
    public interface IReadManga : IManga
    {
        IData[] Illustrators { get; }
        IData[] Magazines { get; }
        IData[] Publishers { get; }
        IData[] ReleaseDate { get; }
        IData[] Writers { get; }
        string Volumes { get; }
    }
}