using libra.core;

namespace libraTest.libretto
{
    public class NestingPartResource : Resource
    {
        public AbstractResourcePart Part { get; set; }
        public NestingPart NestingPart { get; set; }
    }
}