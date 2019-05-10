using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace libretto.model
{
    public class ResourceType
    {
        [JsonProperty("$id")]
        public string Id { get; set; }
        public string Title { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ObjectType Type { get; set; }
        public List<PropertyInfo> Properties { get; set; }
    }
}