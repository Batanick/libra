using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using libra.core;
using libra.core.exceptions;
using libra.core.utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace libraUnity.libra.client
{
    public class JsonLoader : DefaultSerializationBinder
    {
        private readonly Dictionary<string, Type> _typesRegistry = new Dictionary<string, Type>();

        public void Register(List<Type> types)
        {
            foreach (var type in types)
            {
                var name = ReflectionUtils.GetTypeName(type);
                if (_typesRegistry.ContainsKey(name))
                {
                    throw new ResourceSystemException($"Duplicate resource class name: ${name}");
                }

                _typesRegistry.Add(name, type);
            }
        }

        public IEnumerable<Resource> Parse(List<Stream> jsons)
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver(),
                Binder = this,
                TypeNameHandling = TypeNameHandling.Auto,
                MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead
            };

            var result = new List<Resource>(jsons.Count);
            foreach (var stream in jsons)
            {
                result.Add(Parse(stream, settings));
            }

            return result;
        }

        private Resource Parse(Stream stream, JsonSerializerSettings settings)
        {
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                var value = reader.ReadToEnd();
                try
                {
                    return JsonConvert.DeserializeObject<Resource>(value, settings);
                }
                catch (JsonException e)
                {
                    throw new ResourceSystemException("Unable to parse resource", e);
                }
            }
        }

        public override Type BindToType(string assemblyName, string typeName)
        {
            if (_typesRegistry.TryGetValue(typeName, out var type))
            {
                return type;
            }

            return base.BindToType(assemblyName, typeName);
        }
    }
}