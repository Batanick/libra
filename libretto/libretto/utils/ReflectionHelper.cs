using System;
using System.Linq;
using libra.core;
using libra.core.attribs;
using libretto.libretto.exceptions;

namespace libretto.libretto.utils
{
    public static class ReflectionHelper
    {
        public static string GetResourceName(Type resType)
        {
            if (!resType.IsSubclassOf(typeof(Resource)))
            {
                throw new LibrettoException($"Class {resType.Name} is not inherited from {nameof(Resource)}");
            }

            if (resType.IsAbstract)
            {
                throw new LibrettoException($"Cannot calculate the name of {resType.Name}, because it is abstract");
            }

            var customName = resType.GetCustomAttributes(typeof(ResourceNameAttribute), false)
                .Cast<ResourceNameAttribute>()
                .FirstOrDefault();
            return customName == null ? resType.Name : customName.Name;
        }
    }
}