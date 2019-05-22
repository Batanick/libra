using System;
using System.Linq;
using libra.core.attribs;
using libra.core.exceptions;

namespace libra.core.utils
{
    public static class ReflectionUtils
    {
        public static string GetTypeName(Type type)
        {
            if (!type.IsSubclassOf(typeof(Resource)) && !typeof(IResourcePart).IsAssignableFrom(type))
            {
                throw new IllegalResourceClassException(string.Format("Class {0} is neither ResourcePart, nor Resource", type.FullName));
            }

            if (type.IsAbstract || type.IsInterface)
            {
                throw new IllegalResourceClassException(string.Format("Class {0} is an abstract class", type.FullName));
            }
            
            var customName = type.GetCustomAttributes(typeof(ResourceNameAttribute), false)
                .Cast<ResourceNameAttribute>()
                .FirstOrDefault();
            return customName == null ? type.Name : customName.Name;
        }
    }
}