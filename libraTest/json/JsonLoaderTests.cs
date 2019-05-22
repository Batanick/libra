using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using libra.core;
using libra.core.utils;
using libraTest.core;
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
        public void InvalidJson_Throw()
        {
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