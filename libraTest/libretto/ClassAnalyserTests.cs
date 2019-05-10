using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using libra.core;
using libretto.libretto;
using libretto.libretto.model;
using NUnit.Framework;
using static NUnit.Framework.Assert;
using PropertyInfo = libretto.libretto.model.PropertyInfo;

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