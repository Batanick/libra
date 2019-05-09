using System;

namespace libra.core.attribs
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ResourceNameAttribute  : Attribute
    {
        private readonly string name;

        public ResourceNameAttribute(string name)
        {
            this.name = name;
        }
        
        public string Name {
            get {
                return name;
            }
        }
    }
}