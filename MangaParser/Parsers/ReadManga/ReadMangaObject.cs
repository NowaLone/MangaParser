using MangaParser.Core.Interfaces;
using MangaParser.Core.Models;
using System;

namespace MangaParser.Parsers.ReadManga
{
    /// <summary>
    /// Provides an object representation of a "ReadManga" manga with information.
    /// </summary>
    public class ReadMangaObject : IReadManga
    {
        /// <summary>
        ///  Initializes a new instance of the <see cref="ReadMangaObject"/> class with the empty parameters.
        /// </summary>
        public ReadMangaObject()
        {
        }

        public Cover[] Covers { get; set; }
        public IData[] Autors { get; set; }
        public IData[] Genres { get; set; }
        public IData[] Illustrators { get; set; }
        public IData[] Magazines { get; set; }
        public IData[] Publishers { get; set; }
        public IData[] ReleaseDate { get; set; }
        public IData[] Writers { get; set; }
        public Name Name { get; set; }
        public string Description { get; set; }
        public string Volumes { get; set; }
        public Uri MangaUri { get; set; }

        public override string ToString()
        {
            return $"Name: {Name}\n" +
                $"\nDescription: {Description}\n" +
                $"\nAutor: {String.Join(", ", Autors != null ? Autors : new object[0])}\n" +
                $"\nWriter: {String.Join(", ", Writers != null ? Writers : new object[0])}\n" +
                $"\nIllustrator: {String.Join(", ", Illustrators != null ? Illustrators : new object[0])}\n" +
                $"\nPublisher: {String.Join(", ", Publishers != null ? Publishers : new object[0])}\n" +
                $"\nMagazine: {String.Join(", ", Magazines != null ? Magazines : new object[0])}\n" +
                $"\nGenres: {String.Join(", ", Genres != null ? Genres : new object[0])}\n" +
                $"\nVolumes: {Volumes}\n" +
                $"\nRelease date: {String.Join(", ", ReleaseDate != null ? ReleaseDate : new object[0])}\n" +
                $"\nCovers:\n{String.Join("\n---\n", Covers != null ? Covers : new object[0])}\n" +
                $"\nLink: {MangaUri}\n";
        }
    }
}