using libra.core;

namespace libraTest.libretto
{
    public class NestingPart : IResourcePart
    {
        public string TestString { get; set; }
        public NestingPart Part { get; set; }
    }
}