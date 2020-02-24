namespace MangaParser.Core.Interfaces
{
    public interface IMangaObject : IManga
    {
        IData[] Illustrators { get; set; }
        IData[] Magazines { get; set; }
        IData[] Publishers { get; set; }
        IData[] ReleaseDate { get; set; }
        string Volumes { get; set; }
        IData[] Writers { get; set; }
    }
}