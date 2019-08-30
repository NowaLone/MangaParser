using MangaParser.Core.Interfaces;
using MangaParser.Core.Models;
using System;

namespace MangaParser.Parsers.MangaFox
{
    /// <summary>
    /// Provides an object representation of a "MangaFox" manga with information.
    /// </summary>
    public class MangaFoxObject : IManga
    {
        #region Constructors

        /// <summary>
        ///  Initializes a new instance of the <see cref="MangaFoxObject"/> class with the empty parameters.
        /// </summary>
        public MangaFoxObject()
        {
        }

        #endregion Constructors

        #region Properties

        public IData[] Autors { get; set; }
        public IData[] Genres { get; set; }
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
                $"\nGenres: {String.Join(", ", Genres != null ? Genres : new object[0])}\n" +
                $"\nCovers:\n{String.Join("\n---\n", Covers != null ? Covers : new object[0])}\n" +
                $"\nLink: {MangaUri}\n";
        }

        #endregion Methods
    }
}