using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using libretto.libretto;
using libretto.libretto.model;
using NUnit.Framework;
using static NUnit.Framework.Assert;

namespace libraTest.libretto
{
    public class ClassAnalyserTests
    {
        private List<ResourceType> _typeInfo;

        [OneTimeSetUp]
        public void AnalyseThis()
        {
            var classAnalyser = new ClassAnalyser();
            _typeInfo = classAnalyser.Process(new List<Assembly> {typeof(LibraTests).Assembly});
        }

        [Test]
        public void AnalyseAssembly_FoundResources()
        {
            IsNotEmpty(_typeInfo);
            _typeInfo.Select(s => )
        }

        public static string NameOf()
        {
            
        }
    }
}