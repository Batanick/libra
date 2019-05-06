using System.Collections.Generic;
using System.Linq;
using libra.core;
using libra.core.exceptions;
using libraTest.utils;
using NUnit.Framework;
using static libraTest.utils.ResourceGenHelper;
using static NUnit.Framework.Assert;

namespace libraTest.core
{
    public class ResourceSystemTests
    {
        private static readonly ResourceConfigs Config = new ResourceConfigs
        {
            Logger = TestLogger.Create(nameof(ResourceSystemTests))
        };

        [Test]
        public void LoadingSimple()
        {
            ResourceSystem.Create(Config, new List<Resource>
            {
                Dummy(), Silly()
            });
        }

        [Test]
        public void DuplicateId_IgnoresDuplicates()
        {
            var resource = Dummy();
            var resourceDuplicate = Dummy(resource.ResourceId);
            var system = ResourceSystem.Create(Config, new List<Resource>
            {
                resource, Silly(), resourceDuplicate, Dummy()
            });

            AreEqual(resource, system.Resolve(resource.ToRef()));
            IsTrue(system.GetAll<DummyResource>().Contains(resource));
        }

        [Test]
        public void DuplicateIdDiffType_IgnoresDuplicates()
        {
            var resource = Dummy();
            var resourceDuplicate = Grumpy(resource.ResourceId);
            var system = ResourceSystem.Create(Config, new List<Resource>
            {
                resource, Silly(), resourceDuplicate, Dummy()
            });

            AreEqual(resource, system.Resolve(resource.ToRef()));
            AreNotEqual(resourceDuplicate, system.Resolve(resource.ToRef()));
            IsTrue(system.GetAll<DummyResource>().Contains(resource));
            IsFalse(system.GetAll<GrumpyResource>().Contains(resourceDuplicate));
        }

        [Test]
        public void CanResolveByRef()
        {
            var resource = Dummy();
            var system = ResourceSystem.Create(Config, new List<Resource>
            {
                Dummy(), Silly(), resource
            });

            var resolved = system.Resolve(resource.ToRef());
            AreEqual(resource, resolved);
        }

        [Test]
        public void CanResolveRef()
        {
            var resource = Dummy();
            var system = ResourceSystem.Create(Config, new List<Resource>
            {
                Dummy(), Silly(), resource
            });

            var resolved = resource.ToRef().Resolve(system);
            AreEqual(resource, resolved);
        }

        [Test]
        public void ThrowOnNullRef()
        {
            var system = ResourceSystem.Create(Config, new List<Resource>());
            Throws<NullResourceReferenceException>(() => system.Resolve<GrumpyResource>(null));
        }

        [Test]
        public void UnknownType_ReturnsEmpty()
        {
            var system = ResourceSystem.Create(Config, new List<Resource>
            {
                Dummy(), Silly(), Silly()
            });
            var grumpyResources = system.GetAll<GrumpyResource>();
            IsNotNull(grumpyResources);
            IsEmpty(grumpyResources);
        }

        [Test]
        public void ResolveMultiple_ReturnsSome()
        {
            var res1 = Silly();
            var res2 = Silly();
            var res3 = Silly();

            var dummy = Dummy();

            var system = ResourceSystem.Create(Config, new List<Resource>
            {
                res1, dummy, res2, res3
            });
            var sillyResources = system.GetAll<SillyResource>().ToList();
            IsNotNull(sillyResources);
            IsNotEmpty(sillyResources);
            AreEqual(3, sillyResources.Count);

            Contains(res1, sillyResources);
            Contains(res2, sillyResources);
            Contains(res3, sillyResources);
        }
    }
}