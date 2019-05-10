using System;

namespace libra.core.attribs
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PropertyNameAttribute : Attribute
    {
        private readonly string _name;

        public PropertyNameAttribute(string name)
        {
            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }
    }
}