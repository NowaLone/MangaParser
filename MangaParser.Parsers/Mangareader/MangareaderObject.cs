using MangaParser.Core.Interfaces;
using MangaParser.Core.Models;
using System;

namespace MangaParser.Parsers.Mangareader
{
    /// <summary>
    /// Provides an object representation of a "Mangareader" manga with information.
    /// </summary>
    public class MangareaderObject : IManga
    {
        #region Constructors

        /// <summary>
        ///  Initializes a new instance of the <see cref="MangareaderObject"/> class with the empty parameters.
        /// </summary>
        public MangareaderObject()
        {
        }

        #endregion Constructors

        #region Properties

        public IData[] Autors { get; set; }
        public IData[] Illustrators { get; set; }
        public IData[] Genres { get; set; }
        public IData[] ReleaseDate { get; set; }
        public MangaCover[] Covers { get; set; }
        public MangaName Name { get; set; }
        public string Description { get; set; }
        public Uri MangaUri { get; set; }

        #endregion Properties

        #region Methods

        public override string ToString()
        {
            return $"Name: {Name}\n" +
                $"\nDescription: {Description}\n" +
                $"\nAutor: {String.Join(", ", Autors != null ? Autors : new object[0])}\n" +
                $"\nIllustrator: {String.Join(", ", Illustrators != null ? Illustrators : new object[0])}\n" +
                $"\nGenres: {String.Join(", ", Genres != null ? Genres : new object[0])}\n" +
                $"\nRelease date: {String.Join(", ", ReleaseDate != null ? ReleaseDate : new object[0])}\n" +
                $"\nCovers:\n{String.Join("\n---\n", Covers != null ? Covers : new object[0])}\n" +
                $"\nLink: {MangaUri}\n";
        }

        #endregion Methods
    }
}