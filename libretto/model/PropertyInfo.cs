using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace libretto.model
{
    public class PropertyInfo
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public ObjectType Type { get; set; }
        [JsonProperty("allowedTypes")]
        public List<string> AllowedTypes { get; set; }
        [JsonProperty("elements")]
        public PropertyInfo Elements { get; set; }
    }
}