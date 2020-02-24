using System;

namespace MangaParser.Core.Interfaces
{
    public interface IMangaCover
    {
        Uri Large { get; }
        Uri Medium { get; }
        Uri Small { get; }
    }
}