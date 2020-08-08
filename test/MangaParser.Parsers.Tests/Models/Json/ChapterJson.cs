using MangaParser.Core.Interfaces;
using MangaParser.Core.Models;
using Newtonsoft.Json;
using System;

namespace MangaParser.Parsers.Tests.Models.Json
{
    public class ChapterJson : Chapter
    {
        [JsonConstructor]
        public ChapterJson(IName value, string url, DateTime addedDate, ICover cover = null) : base(value, url, addedDate, cover)
        {
        }
    }
}