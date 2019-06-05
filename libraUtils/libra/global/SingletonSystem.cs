using libra.core;

namespace libraUnity.libra.global
{
    public static class SingletonSystem
    {
        public static IResourceSystem Instance;

        public static void Init(IResourceSystem system)
        {
            Instance = system;
        }

        public static T Get<T>(this ResourceRef<T> reference) where T : Resource
        {
            return reference.Resolve(Instance);
        }
    }
}