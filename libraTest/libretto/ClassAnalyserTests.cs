using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using libra.core;
using libretto;
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

            AssertProp(resourceType, nameof(BaseTypesResource.IntField), ObjectType.integer);
            AssertProp(resourceType, nameof(BaseTypesResource.LongField), ObjectType.integer);
            AssertProp(resourceType, nameof(BaseTypesResource.ByteField), ObjectType.integer);

            AssertProp(resourceType, nameof(BaseTypesResource.FloatField), ObjectType.number);
            AssertProp(resourceType, nameof(BaseTypesResource.DoubleField), ObjectType.number);

            AssertProp(resourceType, nameof(BaseTypesResource.StringField), ObjectType.@string);
            AssertProp(resourceType, nameof(BaseTypesResource.BoolField), ObjectType.boolean);
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

            AssertProp(resourceType, nameof(AbstractResource.BaseClassProp), ObjectType.integer);
            AssertProp(resourceType, nameof(DerivedResource.DerivedClassProp), ObjectType.@string);
        }

        [Test]
        public void ReferenceField_Processed()
        {
            var resourceType = _typeInfo.First(r => r.Id == nameof(ReferencingResource));
            var prop = AssertProp(resourceType, nameof(ReferencingResource.Reference), ObjectType.@ref);

            Contains(nameof(DerivedResource), prop.AllowedTypes);
            Contains(nameof(AnotherDerivedResource), prop.AllowedTypes);
            False(prop.AllowedTypes.Contains(nameof(AbstractDerivedResource)));
        }

        [Test]
        public void CustomFieldsInfo_Correct()
        {
            var resourceType = _typeInfo.First(r => r.Id == nameof(CustomPropertiesResource));
            AssertProp(resourceType, nameof(CustomPropertiesResource.JustProperty), ObjectType.integer);

            var custom = AssertProp(resourceType, nameof(CustomPropertiesResource.CustomTitleProperty), ObjectType.boolean);
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
            var propertyInfo = AssertProp(resourceType, nameof(NestingPartResource.Part), ObjectType.obj);
            Contains(nameof(ResourcePart), propertyInfo.AllowedTypes);
            Contains(nameof(ResourcePart2), propertyInfo.AllowedTypes);
            IsFalse(propertyInfo.AllowedTypes.Contains(nameof(AbstractResourcePart)));
        }

        [Test]
        public void ResourcePartFields_Processed()
        {
            var resourceType = _typeInfo.First(r => r.Id == nameof(ResourcePart2));
            AssertProp(resourceType, nameof(ResourcePart2.BoolField), ObjectType.boolean);
            AssertProp(resourceType, nameof(ResourcePart2.IntField), ObjectType.integer);
            AssertProp(resourceType, nameof(ResourcePart2.StringField), ObjectType.@string);
        }

        [Test]
        public void ArrayClass_Processed()
        {
            var resourceType = _typeInfo.First(r => r.Id == nameof(ArrayResource));

            var intList = AssertProp(resourceType, nameof(ArrayResource.IntList), ObjectType.array);
            AreEqual(ObjectType.integer, intList.Elements.Type);
            var partList = AssertProp(resourceType, nameof(ArrayResource.PartList), ObjectType.array);
            AreEqual(ObjectType.obj, partList.Elements.Type);
            var resList = AssertProp(resourceType, nameof(ArrayResource.ResourceList), ObjectType.array);
            AreEqual(ObjectType.@ref, resList.Elements.Type);

            var floatArr = AssertProp(resourceType, nameof(ArrayResource.FloatArray), ObjectType.array);
            AreEqual(ObjectType.number, floatArr.Elements.Type);
            var partArr = AssertProp(resourceType, nameof(ArrayResource.PartArray), ObjectType.array);
            AreEqual(ObjectType.obj, partArr.Elements.Type);
            var resArr = AssertProp(resourceType, nameof(ArrayResource.ResourceArray), ObjectType.array);
            AreEqual(ObjectType.@ref, resArr.Elements.Type);
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