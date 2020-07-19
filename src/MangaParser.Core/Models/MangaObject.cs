using MangaParser.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace MangaParser.Core.Models
{
    /// <summary>
    /// Provides an object representation of a manga with some information.
    /// </summary>
    public class MangaObject : DataBase<IName>, IMangaObject
    {
        #region Constructors

        /// <inheritdoc cref="MangaObject(IName, Uri)"/>
        public MangaObject(IName value, string url) : base(value, url)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MangaObject"/> class with a minimum parameters.
        /// </summary>
        /// <param name="value">A data with a manga name.</param>
        /// <param name="url">An url to the manga.</param>
        public MangaObject(IName value, Uri url) : base(value, url)
        {
        }

        #endregion Constructors

        #region Properties

        public ICollection<ICover> Covers { get; set; }
        public ICollection<IDataBase<IName>> Authors { get; set; }
        public ICollection<IDataBase<IName>> Genres { get; set; }
        public IDataBase<string> Description { get; set; }
        public ICollection<IDataBase<IName>> Illustrators { get; set; }
        public ICollection<IDataBase<IName>> Writers { get; set; }
        public ICollection<IDataBase<IName>> Magazines { get; set; }
        public ICollection<IDataBase<IName>> Publishers { get; set; }
        public IDataBase<DateTime> ReleaseDate { get; set; }
        public IDataBase<int> Volumes { get; set; }

        public string Source => Url != null ? Url.Host : "Unknown";

        #endregion Properties
    }
}