using System;

namespace MangaParser.Core.Interfaces
{
    /// <summary>
    /// Defines a minimum information about a manga chapter.
    /// </summary>
    public interface IChapter
    {
        DateTime AddedDate { get; }
        string Name { get; }
        Uri ChapterUri { get; }
    }
}