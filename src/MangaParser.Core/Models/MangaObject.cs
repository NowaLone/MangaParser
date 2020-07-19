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

        /// <summary>
        /// Initializes a new instance of the <see cref="MangaObject"/> class with a minimum parameters.
        /// </summary>
        /// <param name="value">A data with a manga name.</param>
        /// <param name="url">An url to the manga.</param>
        public MangaObject(IName value, Uri url) : base(value, url)
        {
        }

        /// <inheritdoc cref="MangaObject(IName, Uri)"/>
        public MangaObject(IName value, string url) : base(value, url)
        {
        }

        #endregion Constructors

        #region Properties

        public virtual ICollection<ICover> Covers { get; set; }
        public virtual ICollection<IDataBase<IName>> Authors { get; set; }
        public virtual ICollection<IDataBase<IName>> Genres { get; set; }
        public virtual IDataBase<string> Description { get; set; }
        public virtual ICollection<IDataBase<IName>> Illustrators { get; set; }
        public virtual ICollection<IDataBase<IName>> Writers { get; set; }
        public virtual ICollection<IDataBase<IName>> Magazines { get; set; }
        public virtual ICollection<IDataBase<IName>> Publishers { get; set; }
        public virtual IDataBase<DateTime> ReleaseDate { get; set; }
        public virtual IDataBase<int> Volumes { get; set; }

        public virtual string Source => Url != null ? Url.Host : "Unknown";

        #endregion Properties
    }
}