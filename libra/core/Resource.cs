using System;

namespace libra.core
{
    public abstract class Resource
    {
        public Guid ResourceId { get; set; }

        public bool Equals(Resource other)
        {
            return ResourceId.Equals(other.ResourceId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Resource) obj);
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