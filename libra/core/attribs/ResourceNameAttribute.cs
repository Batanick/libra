using System;

namespace libra.core.attribs
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ResourceNameAttribute  : Attribute
    {
        private readonly string _name;

        public ResourceNameAttribute(string name)
        {
            _name = name;
        }
        
        public string Name {
            get {
                return _name;
            }
        }
    }
}