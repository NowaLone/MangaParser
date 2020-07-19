using System;
using System.Runtime.Serialization;

namespace MangaParser.Core.Exceptions
{
    public class BaseHostNotMatchException : ArgumentException
    {
        #region Fields

        private const string message = "Base url host '{0}' and parameter host {1} don't match.";

        #endregion Fields

        #region Constructors

        public BaseHostNotMatchException()
        {
        }

        public BaseHostNotMatchException(string message) : base(message)
        {
        }

        public BaseHostNotMatchException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public BaseHostNotMatchException(string baseHost, string paramHost, string paramName) : base(string.Format(message, baseHost, paramHost), paramName)
        {
        }

        public BaseHostNotMatchException(string baseHost, string paramHost, string paramName, Exception innerException) : base(string.Format(message, baseHost, paramHost), paramName, innerException)
        {
        }

        protected BaseHostNotMatchException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        #endregion Constructors
    }
}