using System.Collections.Generic;

namespace libretto.libretto.model
{
    public class ResourceType
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public ObjectType Type { get; set; }
        public List<PropertyInfo> Properties { get; set; }
    }
}