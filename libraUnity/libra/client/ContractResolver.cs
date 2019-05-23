using System;
using System.Reflection;
using libra.core;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace libraUnity.libra.client
{
    public class ContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            if (property.DeclaringType == typeof(Resource))
            {
                if (property.PropertyName.Equals(nameof(Resource.ResourceId), StringComparison.OrdinalIgnoreCase))
                {
                    property.Required = Required.Always;
                    property.PropertyName = "id";
                }

                return property;
            }

            return property;
        }
    }
}