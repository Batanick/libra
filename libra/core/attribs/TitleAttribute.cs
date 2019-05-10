using System;

namespace libra.core.attribs
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TitleAttribute : Attribute
    {
        private readonly string _name;

        public TitleAttribute(string name)
        {
            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }
    }
}