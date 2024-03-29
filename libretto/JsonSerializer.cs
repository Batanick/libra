using libretto.model;
using Newtonsoft.Json;

namespace libretto
{
    public class JsonSerializer
    {
        
        public string Serialize(ResourceInfo resType)
        {
            return JsonConvert.SerializeObject(resType, Formatting.Indented, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });    
        }
    }
}