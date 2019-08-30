using MangaParser.Core.Interfaces;
using System;

namespace MangaParser.Core.Models
{
    public class MangaPage : IPage
    {
        #region Constructors

        public MangaPage(Uri pageUri)
        {
            PageUri = pageUri;
        }

        public MangaPage(string pageUri)
        {
            if (Uri.IsWellFormedUriString(pageUri, UriKind.Absolute))
                PageUri = new Uri(pageUri);
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// A manga page URI.
        /// </summary>
        public Uri PageUri { get; }

        #endregion Properties
    }
}