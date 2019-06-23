namespace MangaParser.Core.Models
{
    public class MangaName
    {
        public MangaName(string english = default(string), string localized = default(string), string original = default(string))
        {
            this.English = english;
            this.Localized = localized;
            this.Original = original;
        }
        
        public string English { get; }
        public string Localized { get; }
        public string Original { get; }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Localized))
                return Localized;
            else if (!string.IsNullOrEmpty(English))
                return English;
            else
                return Original;
        }
    }
}