using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using libraUnity.libra.client;
using libretto;
using NUnit.Framework;
using static NUnit.Framework.Assert;

namespace libraTest.json
{
    public class JsonLoaderTests
    {
        private readonly List<Type> _allTypes = new List<Type>();

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
            var loader = new JsonLoader();

            IsNotEmpty(_allTypes);
            loader.Register(_allTypes);

            var allFiles = LoadAll();
            IsNotEmpty(allFiles);
            var resources = loader.Parse(allFiles).ToList();

            IsFalse(resources.Any(r => r == null));
            AreEqual(allFiles.Count, resources.Count);
        }

        [Test]
        public void InvalidJson_Throw()
        {
        }

        private List<Stream> LoadAll()
        {
            var result = new List<Stream>();
            foreach (string res in typeof(LibraTests).Assembly.GetManifestResourceNames())
            {
                if (res.EndsWith(".rs"))
                {
                    result.Add(typeof(LibraTests).Assembly.GetManifestResourceStream(res));
                }
            }

            return result;
        }
    }
}