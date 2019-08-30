namespace MangaParser.Core.Models
{
    public class MangaName
    {
        #region Constructors

        public MangaName(string english = default(string), string localized = default(string), string original = default(string))
        {
            this.English = english;
            this.Localized = localized;
            this.Original = original;
        }

        #endregion Constructors

        #region Properties

        public string English { get; }
        public string Localized { get; }
        public string Original { get; }

        #endregion Properties

        #region Methods

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Localized))
                return Localized;
            else if (!string.IsNullOrEmpty(English))
                return English;
            else
                return Original;
        }

        #endregion Methods
    }
}