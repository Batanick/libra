using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using libra.core;
using libra.core.exceptions;
using libra.core.utils;
using libraTest.core;
using libraTest.libretto;
using libraUnity.libra.client;
using libretto;
using NUnit.Framework;
using static NUnit.Framework.Assert;

namespace libraTest.json
{
    public class JsonLoaderTests
    {
        private readonly List<Type> _allTypes = new List<Type>();
        private readonly IResourceLogger _logger = TestLogger.Create(nameof(JsonLoaderTests));

        [OneTimeSetUp]
        public void SetUp()
        {
            _allTypes.Clear();
            var types = ClassAnalyser.ExtractTypes(new List<Assembly> {typeof(LibraTests).Assembly});
            _allTypes.AddRange(types.Where(ClassAnalyser.IsPart).ToList());
            _allTypes.AddRange(types.Where(ClassAnalyser.IsResource).ToList());
        }

        [Test]
        public void LoadAll_Success()
        {
            var resources = LoadAll();
            IsNotEmpty(resources);
            IsFalse(resources.Any(r => r == null));
        }


        [Test]
        public void BaseFieldTypes_Success()
        {
            var resources = LoadAll();
            var baseResource = resources.First(r => r.ResourceId == Guid.Parse("7d304d1d-da59-4419-9d97-af59fb4cb5e7"));
            IsNotNull(baseResource);
            IsInstanceOf<BaseTypesResource>(baseResource);

            var res = (BaseTypesResource) baseResource;

            IsTrue(res.BoolField);
            AreEqual(123, res.ByteField);
            AreEqual(123, res.DoubleField);
            AreEqual(1231.2211f, res.FloatField);
            AreEqual(123, res.IntField);
            AreEqual(123, res.LongField);
            AreEqual("21312dsadas", res.StringField);
        }

        [Test]
        public void ResourceArray_Success()
        {
            var resources = LoadAll();
            var resource = resources.First(r => r.ResourceId == Guid.Parse("2844969a-3bdb-4bd5-8ecc-4bb6a76edb14"));
            IsNotNull(resource);
            IsInstanceOf<ArrayResource>(resource);

            var res = (ArrayResource) resource;

            AreEqual(new[] {1.0f, 2.0f, 3.0f}, res.FloatArray);
            AreEqual(new[] {1, 2}, res.IntList);

            IsNull(res.ResourceArray);

            IsNotNull(res.ResourceList);
            AreEqual(2, res.ResourceList.Count);
            var resRef = ResourceRef<BaseTypesResource>.Create(Guid.Parse("7d304d1d-da59-4419-9d97-af59fb4cb5e7"));
            AreEqual(new List<ResourceRef<BaseTypesResource>> {resRef, resRef}, res.ResourceList);

            IsNotNull(res.PartArray);
            AreEqual(3, res.PartArray.Length);
            var part = res.PartArray[0];
            IsNotNull(part);
            IsInstanceOf<ResourcePart>(part);
            AreEqual(1, part.IntField);
            AreEqual("2", part.StringField);
            IsNull(res.PartArray[1]);
            IsNull(res.PartArray[2]);

            IsNotNull(res.PartList);
            AreEqual(2, res.PartList.Count);

            var listPart1 = res.PartList[0];
            IsNotNull(listPart1);
            IsInstanceOf<ResourcePart>(listPart1);
            AreEqual(3, listPart1.IntField);
            AreEqual("3", listPart1.StringField);
            var listPart2 = res.PartList[0];
            IsNotNull(listPart2);
            IsInstanceOf<ResourcePart>(listPart2);
            AreEqual(3, listPart2.IntField);
            AreEqual("3", listPart2.StringField);
        }

        [Test]
        public void InvalidJson_Throw()
        {
            var assembly = typeof(LibraTests).Assembly;
            var loader = new JsonLoader(_logger);
            loader.Register(_allTypes);

            var resource = assembly.GetManifestResourceNames().First(r => r.EndsWith("invalid.json"));
            var stream = assembly.GetManifestResourceStream(resource);
            Throws<ResourceSystemException>(() => loader.Parse(stream));
        }

        [Test]
        public void NoId_Throw()
        {
            var assembly = typeof(LibraTests).Assembly;
            var loader = new JsonLoader(_logger);
            loader.Register(_allTypes);

            var resource = assembly.GetManifestResourceNames().First(r => r.EndsWith("no_id.json"));
            var stream = assembly.GetManifestResourceStream(resource);
            Throws<ResourceSystemException>(() => loader.Parse(stream));
        }

        private List<Resource> LoadAll()
        {
            var assembly = typeof(LibraTests).Assembly;
            var loader = new JsonLoader(_logger);
            loader.Register(_allTypes);

            var result = new List<Resource>();
            foreach (string res in assembly.GetManifestResourceNames())
            {
                if (res.EndsWith(".rs"))
                {
                    _logger.LogInfo($"Parsing: {res}");
                    var resource = loader.Parse(assembly.GetManifestResourceStream(res));
                    result.Add(resource);
                }
            }

            return result;
        }
    }
}