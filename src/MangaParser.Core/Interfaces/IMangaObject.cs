using System;
using System.Collections.Generic;

namespace MangaParser.Core.Interfaces
{
    /// <summary>
    /// Defines the manga parameters.
    /// </summary>
    /// <remarks><see cref="IDataBase{IName}.Value"/> represent manga name.</remarks>
    public interface IMangaObject : IDataBase<IName>
    {
        #region Properties

        /// <summary>
        /// Gets the manga covers collection.
        /// </summary>
        /// <returns>The manga covers collection.</returns>
        ICollection<ICover> Covers { get; set; }

        /// <summary>
        /// Gets the manga authors collection.
        /// </summary>
        /// <returns>The manga authors collection.</returns>
        ICollection<IDataBase<IName>> Authors { get; set; }

        /// <summary>
        /// Gets the manga genres collection.
        /// </summary>
        /// <returns>The manga genres collection.</returns>
        ICollection<IDataBase<IName>> Genres { get; set; }

        /// <summary>
        /// Gets the data with a manga description.
        /// </summary>
        /// <returns>The data with a manga description.</returns>
        IDataBase<string> Description { get; set; }

        /// <summary>
        /// Gets the manga illustrators collection.
        /// </summary>
        /// <returns>The manga illustrators collection.</returns>
        ICollection<IDataBase<IName>> Illustrators { get; set; }

        /// <summary>
        /// Gets the manga writers collection.
        /// </summary>
        /// <returns>The manga writers collection.</returns>
        ICollection<IDataBase<IName>> Writers { get; set; }

        /// <summary>
        /// Gets the manga magazines collection.
        /// </summary>
        /// <returns>The manga magazines collection.</returns>
        ICollection<IDataBase<IName>> Magazines { get; set; }

        /// <summary>
        /// Gets the manga publishers collection.
        /// </summary>
        /// <returns>The manga publishers collection.</returns>
        ICollection<IDataBase<IName>> Publishers { get; set; }

        /// <summary>
        /// Gets the manga release date.
        /// </summary>
        /// <returns>The manga release date.</returns>
        IDataBase<DateTime> ReleaseDate { get; set; }

        /// <summary>
        /// Gets the manga volumes count.
        /// </summary>
        /// <returns>The manga volumes count.</returns>
        IDataBase<int> Volumes { get; set; }

        /// <summary>
        /// Gets the manga url host.
        /// </summary>
        /// <returns>The manga url host.</returns>
        string Source { get; }

        #endregion Properties
    }
}