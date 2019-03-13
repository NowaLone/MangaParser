using MangaParser.Interfaces;
using MangaParser.Models.MangaData;
using System;
using System.Collections.Generic;

namespace MangaParser.Models
{
    /// <summary>
    /// Provides an object representation of a manga with minimum information.
    /// </summary>
    public class MangaTile : IManga
    {
        public string EnglishTitle { get; }
        public string LocalizedTitle { get; }
        public string MangaUrl { get; }
        public List<DataBase> Autors { get; }
        public List<DataBase> Genres { get; }
        public List<Cover> Covers { get; }

        /// <summary>
        ///  Initializes a new instance of the <see cref="MangaTile"/> class with the empty parameters.
        /// </summary>
        public MangaTile()
        {
            Autors = new List<DataBase>();
            Genres = new List<DataBase>();
            Covers = new List<Cover>();
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="MangaTile"/> class with the specified parameters.
        /// </summary>
        /// <param name="engTitle">English title.</param>
        /// <param name="localTitle">Localized title.</param>
        /// <param name="mangaUrl">Manga link.</param>
        public MangaTile(string engTitle, string localTitle, string mangaUrl) : this()
        {
            EnglishTitle = engTitle;
            LocalizedTitle = localTitle;
            MangaUrl = mangaUrl;
        }

        public override string ToString()
        {
            return $"\nEnglish title: {EnglishTitle}\n" +
                $"Title: {LocalizedTitle}\n" +
                $"\nAutor: {String.Join(", ", Autors)}\n" +
                $"\nGenres: {String.Join(", ", Genres)}\n" +
                $"\nCovers:\n{String.Join("\n---\n", Covers)}\n" +
                $"\nLink: {MangaUrl}\n";
        }
    }
}