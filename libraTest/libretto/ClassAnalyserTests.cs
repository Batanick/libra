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
            Contains("Grumpy", _typeInfo.Select(t => t.Id ).ToList());
            Contains(nameof(NamelessResource), _typeInfo.Select(t => t.Id ).ToList());
        }

        [Test]
        public void SimpleFields_Analysed()
        {
            var resourceType = _typeInfo.First(r => r.Id == nameof(BaseTypesResource));
            
            AssertProp(resourceType, nameof(BaseTypesResource.IntField), ObjectType.Integer);
            AssertProp(resourceType, nameof(BaseTypesResource.LongField), ObjectType.Integer);
            AssertProp(resourceType, nameof(BaseTypesResource.ByteField), ObjectType.Integer);
            
            AssertProp(resourceType, nameof(BaseTypesResource.FloatField), ObjectType.Number);
            AssertProp(resourceType, nameof(BaseTypesResource.DoubleField), ObjectType.Number);
            
            AssertProp(resourceType, nameof(BaseTypesResource.StringField), ObjectType.String);
            AssertProp(resourceType, nameof(BaseTypesResource.BoolField), ObjectType.Boolean);
        }

        private static void AssertProp(ResourceType type, string propName, ObjectType objType)
        {
            var props = type.Properties;
            IsNotNull(props);
            IsNotEmpty(props);

            var propInfos = props.Where(p => p.Name == propName).ToList();
            IsNotEmpty(propInfos);
            AreEqual(1, propInfos.Count);

            var prop = propInfos.First();
            AreEqual(propName, prop.Name);
            AreEqual(objType, prop.Type);
        } 
    }
}