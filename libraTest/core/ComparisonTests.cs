using libraTest.utils;
using NUnit.Framework;
using static NUnit.Framework.Assert;

namespace libraTest.core
{
    public class ComparisonTests
    {
        [Test]
        public void SameResourceId_Eq()
        {
            var resource1 = ResourceGenHelper.Silly();
            var resource2 = ResourceGenHelper.Silly(resource1.ResourceId);

            AreEqual(resource1, resource2);
        }

        [Test]
        public void SameIdDiffType_NotEq()
        {
            var resource1 = ResourceGenHelper.Silly();
            var resource2 = ResourceGenHelper.Grumpy(resource1.ResourceId);

            AreNotEqual(resource1, resource2);
        }

        [Test]
        public void DiffId_NotEq()
        {
            var resource1 = ResourceGenHelper.Silly();
            var resource2 = ResourceGenHelper.Silly();

            AreNotEqual(resource1, resource2);
        }
        
        [Test]
        public void RefSameId_Eq()
        {
            var resource1 = ResourceGenHelper.Silly();
            var resource2 = ResourceGenHelper.Silly(resource1.ResourceId);

            AreEqual(resource1.ToRef(), resource2.ToRef());
        }
        
        [Test]
        public void RefSameIdDiffType_NotEq()
        {
            var resource1 = ResourceGenHelper.Silly();
            var resource2 = ResourceGenHelper.Grumpy(resource1.ResourceId);

            AreEqual(resource1.ToRef(), resource2.ToRef());
        }
        
        [Test]
        public void RefDiffId_NotEq()
        {
            var resource1 = ResourceGenHelper.Silly();
            var resource2 = ResourceGenHelper.Silly();

            AreNotEqual(resource1.ToRef(), resource2.ToRef());
        }
    }
}