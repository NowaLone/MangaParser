using System;

namespace MangaParser.Models.MangaData
{
    /// <summary>
    /// Provides an object representation of a manga cover.
    /// </summary>
    public class Cover
    {
        /// <summary>
        /// Gets a low resolution cover URI.
        /// </summary>
        public Uri Small { get; private set; }
        /// <summary>
        ///  Gets a medium resolution cover URI.
        /// </summary>
        public Uri Medium { get; private set; }
        /// <summary>
        /// Gets a high resolution cover URI.
        /// </summary>
        public Uri Large { get; private set; }

        private const string lowRes = "_p";
        private const string mediumRes = "";
        private const string highRes = "_o";

        /// <summary>
        ///  Initializes a new instance of the <see cref="Cover"/> class with the specified URI.
        /// </summary>
        /// <param name="uri">A cover uri.</param>
        public Cover(Uri uri)
        {
            Create(uri);
        }
        /// <summary>
        ///  Initializes a new instance of the <see cref="Cover"/> class with the specified URI.
        /// </summary>
        /// <param name="uri">A cover uri.</param>
        public Cover(string uri)
        {
            if (!Uri.IsWellFormedUriString(uri, UriKind.Absolute))
                return;

            Create(new Uri(uri));
        }
        public Cover(string small, string medium, string large)
        {
            if (Uri.IsWellFormedUriString(small, UriKind.Absolute))
                Small = new Uri(small);

            if (Uri.IsWellFormedUriString(medium, UriKind.Absolute))
                Medium = new Uri(medium);

            if (Uri.IsWellFormedUriString(large, UriKind.Absolute))
                Large = new Uri(large);
        }

        private void Create(Uri uri)
        {
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            if (uri.Segments[uri.Segments.Length - 1].Contains(highRes))
            {
                Small = Replace(uri, highRes, lowRes);
                Medium = Replace(uri, highRes, mediumRes);
                Large = uri;
            }
            else if (uri.Segments[uri.Segments.Length - 1].Contains(lowRes))
            {
                Small = uri;
                Medium = Replace(uri, lowRes, mediumRes);
                Large = Replace(uri, lowRes, highRes);
            }
            else
            {
                Small = Insert(uri, 3, lowRes);
                Medium = uri;
                Large = Insert(uri, 3, highRes);
            }
        }

        private Uri Insert(Uri uri, int pos, string text)
        {
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            var segments = uri.Segments;

            if (segments != null && segments.Length > 0)
            {
                segments[segments.Length - 1] = segments[segments.Length - 1].Insert(pos, text);

                UriBuilder builder = new UriBuilder(uri)
                {
                    Path = String.Concat(segments)
                };

                return builder.Uri;
            }
            else
            {
                return null;
            }
        }

        private Uri Replace(Uri uri, string oldValue, string newValue)
        {
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            if (oldValue == null)
            {
                throw new ArgumentNullException(nameof(oldValue));
            }

            var segments = uri.Segments;

            if (segments != null && segments.Length > 0)
            {
                segments[segments.Length - 1] = segments[segments.Length - 1].Replace(oldValue, newValue);

                UriBuilder builder = new UriBuilder(uri)
                {
                    Path = String.Concat(segments)
                };

                return builder.Uri;
            }
            else
            {
                return null;
            }
        }

        public override string ToString()
        {
            return $"Small: {Small?.OriginalString}\nMedium: {Medium?.OriginalString}\nLarge: {Large?.OriginalString}";
        }
    }
}