using MangaParser.Core.Models;
using Newtonsoft.Json;

namespace MangaParser.Parsers.Tests.Models.Json
{
    public class DataBaseJson<T> : DataBase<T>
    {
        [JsonConstructor]
        public DataBaseJson(T value, string url) : base(value, url)
        {
        }
    }

    public class DataBaseJson : DataBase
    {
        [JsonConstructor]
        public DataBaseJson(object value, string url) : base(value, url)
        {
        }
    }
}