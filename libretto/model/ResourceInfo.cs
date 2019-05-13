using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace libretto.model
{
    public class ResourceInfo
    {
        [JsonProperty("$id")] public string Id { get; set; }

        [JsonProperty("$type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public ResourceType Type { get; set; }

        [JsonProperty("title")] public string Title { get; set; }

        [JsonProperty("properties")] public List<PropertyInfo> Properties { get; set; }
    }
}