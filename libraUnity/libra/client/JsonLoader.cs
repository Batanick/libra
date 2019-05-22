using System;
using System.Collections.Generic;
using System.IO;
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

        private readonly JsonSerializerSettings _settings;
        private readonly IResourceLogger _logger;

        public JsonLoader(IResourceLogger logger)
        {
            _logger = logger;
            _settings = new JsonSerializerSettings
            {
                ContractResolver = new ContractResolver(),
                Converters = new List<JsonConverter> {new ResourceRefConverter()},
                Binder = this,
                TypeNameHandling = TypeNameHandling.Auto,
                MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead
            };
        }

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

        public Resource Parse(Stream stream)
        {
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                var value = reader.ReadToEnd();
                try
                {
                    return JsonConvert.DeserializeObject<Resource>(value, _settings);
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

            _logger.LogWarn($"Unable to resolve type-name: {typeName}");
            return base.BindToType(assemblyName, typeName);
        }
    }
}