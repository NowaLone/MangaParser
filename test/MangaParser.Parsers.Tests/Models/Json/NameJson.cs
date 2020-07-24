using MangaParser.Core.Interfaces;
using MangaParser.Core.Models;
using Newtonsoft.Json;

namespace MangaParser.Parsers.Tests.Models.Json
{
    public class NameJson : Name
    {
        [JsonConstructor]
        public NameJson(IDataBase<string> localized = null, IDataBase<string> english = null, IDataBase<string> original = null) : base(localized, english, original)
        {
        }
    }
}