namespace MangaParser.Core.Models
{
    public class MangaName
    {
        public MangaName(string original = default(string), string english = default(string), string localized = default(string))
        {
            Original = original;
            English = english;
            Localized = localized;
        }

        public string Original { get; }
        public string English { get; }
        public string Localized { get; }

        public override string ToString()
        {
            return $"Original name: {Original}\n" + $"\nEnglish name: {English}\n" + $"\nLocalized name: {Localized}";
        }
    }
}