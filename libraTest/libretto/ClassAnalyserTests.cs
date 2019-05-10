using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using libra.core;
using libretto.libretto;
using libretto.model;
using NUnit.Framework;
using static NUnit.Framework.Assert;
using PropertyInfo = libretto.model.PropertyInfo;

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
            Contains("Grumpy", _typeInfo.Select(t => t.Id).ToList());
            Contains(nameof(NamelessResource), _typeInfo.Select(t => t.Id).ToList());
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

        [Test]
        public void ResourceIdField_Ignored()
        {
            var resourceType = _typeInfo.First(r => r.Id == nameof(BaseTypesResource));
            IsNotNull(resourceType.Properties);
            IsFalse(resourceType.Properties.Any(p => p.Name == nameof(Resource.ResourceId)));
        }

        [Test]
        public void AbstractResource_Ignored()
        {
            IsFalse(_typeInfo.Any(r => r.Id == nameof(AbstractResource)));
        }

        [Test]
        public void DerivedClass_ContainsFields()
        {
            var resourceType = _typeInfo.First(r => r.Id == nameof(DerivedResource));

            AssertProp(resourceType, nameof(AbstractResource.BaseClassProp), ObjectType.Integer);
            AssertProp(resourceType, nameof(DerivedResource.DerivedClassProp), ObjectType.String);
        }

        [Test]
        public void ReferenceField_Processed()
        {
            var resourceType = _typeInfo.First(r => r.Id == nameof(ReferencingResource));
            var prop = AssertProp(resourceType, nameof(ReferencingResource.Reference), ObjectType.Ref);

            Contains(nameof(DerivedResource), prop.AllowedTypes);
            Contains(nameof(AnotherDerivedResource), prop.AllowedTypes);
            False(prop.AllowedTypes.Contains(nameof(AbstractDerivedResource)));
        }

        [Test]
        public void CustomFieldsInfo_Correct()
        {
            var resourceType = _typeInfo.First(r => r.Id == nameof(CustomPropertiesResource));
            AssertProp(resourceType, nameof(CustomPropertiesResource.JustProperty), ObjectType.Integer);

            var custom = AssertProp(resourceType, nameof(CustomPropertiesResource.CustomTitleProperty), ObjectType.Boolean);
            AreEqual("CustomTitle", custom.Title);

            IsFalse(resourceType.Properties.Any(p => p.Name == nameof(CustomPropertiesResource.IgnoredProperty)));
        }

        [Test]
        public void ResourcePartsInfo_Processed()
        {
            Contains(nameof(ResourcePart), _typeInfo.Select(r => r.Id).ToList());
            Contains(nameof(ResourcePart2), _typeInfo.Select(r => r.Id).ToList());
            IsFalse(_typeInfo.Select(r => r.Id).Contains(nameof(AbstractResourcePart)));
        }

        [Test]
        public void PartField_Analysed()
        {
            var resourceType = _typeInfo.First(r => r.Id == nameof(NestingPartResource));
            var propertyInfo = AssertProp(resourceType, nameof(NestingPartResource.Part), ObjectType.Object);
            Contains(nameof(ResourcePart), propertyInfo.AllowedTypes);
            Contains(nameof(ResourcePart2), propertyInfo.AllowedTypes);
            IsFalse(propertyInfo.AllowedTypes.Contains(nameof(AbstractResourcePart)));
        }

        [Test]
        public void ResourcePartFields_Processed()
        {
            var resourceType = _typeInfo.First(r => r.Id == nameof(ResourcePart2));
            AssertProp(resourceType, nameof(ResourcePart2.BoolField), ObjectType.Boolean);
            AssertProp(resourceType, nameof(ResourcePart2.IntField), ObjectType.Integer);
            AssertProp(resourceType, nameof(ResourcePart2.StringField), ObjectType.String);
        }
        
        [Test]
        public void ArrayClass_Processed()
        {
            var resourceType = _typeInfo.First(r => r.Id == nameof(ArrayResource));

            var intList = AssertProp(resourceType, nameof(ArrayResource.IntList), ObjectType.Array);
            AreEqual(ObjectType.Integer, intList.Elements.Type);
            var partList = AssertProp(resourceType, nameof(ArrayResource.PartList), ObjectType.Array);
            AreEqual(ObjectType.Object, partList.Elements.Type);
            var resList = AssertProp(resourceType, nameof(ArrayResource.ResourceList), ObjectType.Array);
            AreEqual(ObjectType.Ref, resList.Elements.Type);
            
            var floatArr = AssertProp(resourceType, nameof(ArrayResource.FloatArray), ObjectType.Array);
            AreEqual(ObjectType.Number, floatArr.Elements.Type);
            var partArr = AssertProp(resourceType, nameof(ArrayResource.PartArray), ObjectType.Array);
            AreEqual(ObjectType.Object, partArr.Elements.Type);
            var resArr = AssertProp(resourceType, nameof(ArrayResource.ResourceArray), ObjectType.Array);
            AreEqual(ObjectType.Ref, resArr.Elements.Type);
        }

        private static PropertyInfo AssertProp(ResourceType type, string propName, ObjectType objType)
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

            return prop;
        }
    }
}