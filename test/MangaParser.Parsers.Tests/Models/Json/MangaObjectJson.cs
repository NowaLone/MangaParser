using MangaParser.Core.Interfaces;
using MangaParser.Core.Models;
using Newtonsoft.Json;

namespace MangaParser.Parsers.Tests.Models.Json
{
    public class MangaObjectJson : MangaObject
    {
        [JsonConstructor]
        public MangaObjectJson(IName value, string url) : base(value, url)
        {
        }
    }
}