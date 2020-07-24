using MangaParser.Core.Interfaces;
using MangaParser.Core.Models;
using Newtonsoft.Json;

namespace MangaParser.Parsers.Tests.Models.Json
{
    public class CoverJson : Cover
    {
        [JsonConstructor]
        public CoverJson(IDataBase large = null, IDataBase medium = null, IDataBase small = null) : base(large, medium, small)
        {
        }
    }
}