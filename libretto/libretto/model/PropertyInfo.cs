using System.Collections.Generic;

namespace libretto.libretto.model
{
    public class PropertyInfo
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public ObjectType Type { get; set; }
        public List<string> AllowedTypes { get; set; }
        public PropertyInfo Elements { get; set; }
    }
}