using MangaParser.Core.Interfaces;
using System;

namespace MangaParser.Core.Models
{
    public class MangaPage : IPage
    {
        public MangaPage(Uri pageUri)
        {
            PageUri = pageUri;
        }

        public MangaPage(string pageUri)
        {
            if (Uri.IsWellFormedUriString(pageUri, UriKind.Absolute))
                PageUri = new Uri(pageUri);
        }

        /// <summary>
        /// A manga page URI.
        /// </summary>
        public Uri PageUri { get; }
    }
}