using System;
using System.Collections.Generic;
using System.Linq;
using libra.core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace libraUnity.libra.client
{
    public class ResourceRefConverter : JsonConverter
    {
        public override bool CanRead => true;
        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("Unnecessary because CanWrite is false. The type will skip the converter.");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }
            
            if (reader.TokenType != JsonToken.String)
            {
                throw new JsonReaderException($"Incorrect token type for ResourceRef: {reader.TokenType}");
            }

            var token = JToken.Load(reader);
            if (!Guid.TryParse(token.Value<string>(), out var guid))
            {
                throw new JsonReaderException($"Incorrect Guid format: {token}");
            }

            var createMethod = objectType.GetMethod(nameof(ResourceRef<Resource>.Create));
            var result = createMethod.Invoke(null, new object[] {guid});
            return result;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(ResourceRef<>);
        }
    }
}