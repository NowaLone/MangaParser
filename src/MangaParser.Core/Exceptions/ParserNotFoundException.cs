using System;

namespace MangaParser.Core.Exceptions
{
    public class ParserNotFoundException : Exception
    {
        #region Fields

        private const string messageBase = "Could't find a suitable parser";
        private const string messageParameter = messageBase + " for parameter '{0}'.";
        private const string messageUrlHost = messageBase + " with base url '{0}'.";

        #endregion Fields

        #region Constructors

        public ParserNotFoundException()
        {
        }

        public ParserNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public ParserNotFoundException(string parameter) : base(String.Format(messageParameter, parameter))
        {
        }

        public ParserNotFoundException(Uri url) : base(String.Format(messageUrlHost, url?.Host))
        {
        }

        #endregion Constructors
    }
}