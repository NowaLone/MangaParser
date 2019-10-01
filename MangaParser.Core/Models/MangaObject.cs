using MangaParser.Core.Interfaces;
using System;

namespace MangaParser.Core.Models
{
    /// <summary>
    /// Provides an object representation of a manga with some information.
    /// </summary>
    [Serializable]
    public class MangaObject : IManga
    {
        #region Constructors

        /// <summary>
        ///  Initializes a new instance of the <see cref="MangaObject"/> class with the empty parameters.
        /// </summary>
        public MangaObject()
        {
        }

        #endregion Constructors

        #region Properties

        public IData[] Autors { get; set; }
        public IData[] Genres { get; set; }
        public IData[] Illustrators { get; set; }
        public IData[] Magazines { get; set; }
        public IData[] Publishers { get; set; }
        public IData[] ReleaseDate { get; set; }
        public IData[] Writers { get; set; }
        public MangaCover[] Covers { get; set; }
        public MangaName Name { get; set; }
        public string Description { get; set; }
        public string Volumes { get; set; }
        public Uri MangaUri { get; set; }

        #endregion Properties

        #region Methods

        public override string ToString()
        {
            return $"Name: {Name}\n" +
                $"\nDescription: {Description}\n" +
                $"\nAutor: {String.Join(", ", Autors ?? (new object[0]))}\n" +
                $"\nWriter: {String.Join(", ", Writers ?? (new object[0]))}\n" +
                $"\nIllustrator: {String.Join(", ", Illustrators ?? (new object[0]))}\n" +
                $"\nPublisher: {String.Join(", ", Publishers ?? (new object[0]))}\n" +
                $"\nMagazine: {String.Join(", ", Magazines ?? (new object[0]))}\n" +
                $"\nGenres: {String.Join(", ", Genres ?? (new object[0]))}\n" +
                $"\nVolumes: {Volumes}\n" +
                $"\nRelease date: {String.Join(", ", ReleaseDate ?? (new object[0]))}\n" +
                $"\nCovers:\n{String.Join(Environment.NewLine, Covers ?? (new object[0]))}\n" +
                $"\nLink: {MangaUri}\n";
        }

        #endregion Methods
    }
}