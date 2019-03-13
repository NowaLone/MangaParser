using MangaParser.Interfaces;
using MangaParser.Models.MangaData;
using System;
using System.Collections.Generic;

namespace MangaParser.Models
{
    /// <summary>
    /// Provides an object representation of a manga with information.
    /// </summary>
    public class MangaObject : IManga
    {
        public string Title { get; }
        public string EnglishTitle { get; }
        public string LocalizedTitle { get; }
        public string Description { get; }
        public string Volumes { get; }
        public List<DataBase> Autors { get; }
        public List<DataBase> Writers { get; }
        public List<DataBase> Illustrators { get; }
        public List<DataBase> Publishers { get; }
        public List<DataBase> Magazines { get; }
        public List<DataBase> Genres { get; }
        public List<DataBase> ReleaseDate { get; }
        public List<Cover> Covers { get; }
        public List<ChapterData> Chapters { get; }

        /// <summary>
        ///  Initializes a new instance of the <see cref="MangaObject"/> class with the empty parameters.
        /// </summary>
        public MangaObject()
        {
            Autors = new List<DataBase>();
            Writers = new List<DataBase>();
            Illustrators = new List<DataBase>();
            Publishers = new List<DataBase>();
            Magazines = new List<DataBase>();
            Genres = new List<DataBase>();
            ReleaseDate = new List<DataBase>();
            Covers = new List<Cover>();
            Chapters = new List<ChapterData>();
        }
        /// <summary>
        ///  Initializes a new instance of the <see cref="MangaObject"/> class with the specified parameters.
        /// </summary>
        /// <param name="title">Original title.</param>
        /// <param name="engTitle">English title.</param>
        /// <param name="localTitle">Localized title.</param>
        /// <param name="description">Description.</param>
        /// <param name="volumes">Volumes released.</param>
        public MangaObject(string title, string engTitle, string localTitle, string description, string volumes) : this()
        {
            Title = title;
            EnglishTitle = engTitle;
            LocalizedTitle = localTitle;
            Description = description;
            Volumes = volumes;
        }

        public override string ToString()
        {
            return $"Title: {Title}\n" +
                $"\nEnglish title: {EnglishTitle}\n" +
                $"\nLocalized title: {LocalizedTitle}\n" +
                $"\nDescription: {Description}\n" +
                $"\nAutor: {String.Join(", ", Autors)}\n" +
                $"\nWriter: {String.Join(", ", Writers)}\n" +
                $"\nIllustrator: {String.Join(", ", Illustrators)}\n" +
                $"\nPublisher: {String.Join(", ", Publishers)}\n" +
                $"\nMagazine: {String.Join(", ", Magazines)}\n" +
                $"\nGenres: {String.Join(", ", Genres)}\n" +
                $"\nVolumes: {Volumes}\n" +
                $"\nRelease date: {String.Join(", ", ReleaseDate)}\n" +
                $"\nCovers:\n{String.Join("\n---\n", Covers)}\n" +
                $"\nChapters:\n{String.Join("\n", Chapters)}\n";
        }
    }
}