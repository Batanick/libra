using libra.core;

namespace libraTest.libretto
{
    public class ReferencingResource : Resource
    {
        public ResourceRef<AbstractResource> Reference { get; set; }
    }
}