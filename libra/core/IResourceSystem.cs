using System.Collections.Generic;

namespace libra.core
{
    public interface IResourceSystem
    {
        T Resolve<T>(ResourceRef<T> reference) where T : Resource;
        IEnumerable<T> GetAll<T>() where T : Resource;
    }
}