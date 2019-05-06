using System;

namespace libra.core
{
    public class ResourceRef<T> where T : Resource
    {
        private ResourceRef(Guid resourceId)
        {
            ResourceId = resourceId;
        }

        public static ResourceRef<T> Create(Guid guid)
        {
            return new ResourceRef<T>(guid);
        }

        public Guid ResourceId { get; private set; }

        public T Resolve(IResourceSystem resourceSystem)
        {
            return resourceSystem.Resolve(this);
        }

        public bool Equals(ResourceRef<T> other)
        {
            return ResourceId.Equals(other.ResourceId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ResourceRef<T>) obj);
        }

        public override int GetHashCode()
        {
            return ResourceId.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("ResourceId: {0}", ResourceId);
        }
    }
}