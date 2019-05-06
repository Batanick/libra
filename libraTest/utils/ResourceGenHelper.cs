using System;
using libra.core;

namespace libraTest.utils
{
    public static class ResourceGenHelper
    {
        public static Resource Silly(Guid guid)
        {
            return new SillyResource
            {
                ResourceId = guid
            };
        }

        public static Resource Silly()
        {
            return Silly(Guid.NewGuid());
        }
        
        public static Resource Grumpy(Guid guid)
        {
            return new GrumpyResource
            {
                ResourceId = guid
            };
        }

        public static Resource Grumpy()
        {
            return Grumpy(Guid.NewGuid());
        }

        public static Resource Dummy(Guid guid)
        {
            return new DummyResource
            {
                ResourceId = guid
            };
        }

        public static Resource Dummy()
        {
            return Dummy(Guid.NewGuid());
        }
    }
}