using MangaParser.Core.Interfaces;
using MangaParser.Parsers.Tests.Models.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MangaParser.Parsers.Tests.Converters
{
    public class FromJsonToDataConverter : JsonConverter
    {
        /// <summary>
        /// A key is an <see cref="MangaParser.Core.Interfaces"/> definition and a value is a <see cref="MangaParser.Parsers.Tests.Models.Json"/> realization.
        /// </summary>
        private readonly IDictionary<Type, Type> types;

        public FromJsonToDataConverter()
        {
            types = new Dictionary<Type, Type>
            {
                { typeof(IMangaObject), typeof(MangaObjectJson) },
                { typeof(ICover),       typeof(CoverJson) },
                { typeof(IName),        typeof(NameJson) },
                { typeof(IChapter),     typeof(ChapterJson) },
                { typeof(IDataBase),    typeof(DataBaseJson) },
                { typeof(IDataBase<>),  typeof(DataBaseJson<>) }
            };
        }

        public FromJsonToDataConverter(IDictionary<Type, Type> types)
        {
            this.types = types;
        }

        public override bool CanConvert(Type objectType)
        {
            if (objectType.IsGenericType)
            {
                return types.Any(t => t.Key == objectType.GetGenericTypeDefinition());
            }
            else
            {
                return types.Any(t => t.Key == objectType);
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            if (objectType.IsGenericType)
            {
                var type = types[objectType.GetGenericTypeDefinition()].MakeGenericType(objectType.GenericTypeArguments);
                return serializer.Deserialize(reader, type);
            }
            else
            {
                return serializer.Deserialize(reader, types[objectType]);
            }
        }

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}