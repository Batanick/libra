using libra.core;

namespace libraTest.utils
{
    public static class ResourceHelper
    {
        public static ResourceRef<T> ToRef<T>(this T res) where T : Resource
        {
            return ResourceRef<T>.Create(res.ResourceId);
        }
    }
}