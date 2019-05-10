using libretto.model;
using Newtonsoft.Json;

namespace libretto
{
    public class JsonSerializer
    {
        
        public string Serialize(ResourceType resType)
        {
            return JsonConvert.SerializeObject(resType, Formatting.Indented);    
        }
    }
}