using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using libra.core;
using libretto.libretto.exceptions;
using libretto.libretto.model;
using libretto.libretto.utils;
using NLog;
using PropertyInfo = libretto.libretto.model.PropertyInfo;

namespace libretto.libretto
{
    public class ClassAnalyser
    {
        private static Logger _log = LogManager.GetLogger(nameof(ClassAnalyser));

        private static readonly Dictionary<Type, ObjectType> PrimitiveTypesMapping = new Dictionary<Type, ObjectType>
        {
            {typeof(long), ObjectType.Integer},
            {typeof(int), ObjectType.Integer},
            {typeof(byte), ObjectType.Integer},

            {typeof(float), ObjectType.Number},
            {typeof(double), ObjectType.Number},

            {typeof(string), ObjectType.String},
            {typeof(bool), ObjectType.Boolean}
        };

        private readonly List<Type> _resources = new List<Type>();
        private readonly List<Type> _parts = new List<Type>();

        public List<ResourceType> Process(List<Assembly> assemblies)
        {
            var types = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => !t.IsAbstract)
                .Where(t => !t.IsInterface)
                .ToList();

            CheckTypes(types);

            var result = new List<ResourceType>();
            _resources.AddRange(types.Where(IsResource));
            foreach (var res in _resources)
            {
                result.Add(ProcessType(res, true));
            }

            _parts.AddRange(types.Where(IsPart));
            foreach (var part in _parts)
            {
                result.Add(ProcessType(part, false));
            }

            return result;
        }

        private ResourceType ProcessType(Type t, bool resource)
        {
            _log.Info($"Processing: {t.Name}");
            var name = ReflectionHelper.GetResourceName(t);

            var props = new List<PropertyInfo>();
            foreach (var info in t.GetProperties())
            {
                if (ReflectionHelper.Ignored(info))
                {
                    continue;
                }
                
                if (resource && info.Name == nameof(Resource.ResourceId))
                {
                    continue;
                }
                
                var processed = ProcessInfoProperty(info);
                if (processed != null)
                {
                    props.Add(processed);
                }
            }

            return new ResourceType
            {
                Id = name,
                Properties = props
            };
        }

        private PropertyInfo ProcessInfoProperty(System.Reflection.PropertyInfo info)
        {
            var title = ReflectionHelper.GetPropertyTitle(info);
            var name = info.Name;

            if (info.PropertyType.IsGenericType && info.PropertyType.GetGenericTypeDefinition() == typeof(ResourceRef<>))
            {
                var resType = info.PropertyType.GetGenericArguments()[0];
                var compatibleTypes = GetCompatibleTypes(_resources, resType);
                if (compatibleTypes.Count == 0)
                {
                    _log.Warn($"Unable to find potential reference candidates for: {resType.Name}, ignoring");
                    return null;
                }
                
                return new PropertyInfo
                {
                    Name = name,
                    Type = ObjectType.Ref,
                    Title = title,
                    AllowedTypes = compatibleTypes
                };
            }
            
            if (PrimitiveTypesMapping.TryGetValue(info.PropertyType, out var type))
            {
                return new PropertyInfo
                {
                    Name = name,
                    Title = title,
                    Type = type
                };
            }

            throw new LibrettoException($"Unable process property: {info.DeclaringType.FullName}/{info.Name}");
        }

        private List<string> GetCompatibleTypes(List<Type> types, Type expected)
        {
            return types
                .Where(expected.IsAssignableFrom)
                .Select(ReflectionHelper.GetResourceName)
                .ToList();
        }

        private void CheckTypes(List<Type> types)
        {
            var brokenTypes = types
                .Where(t => IsPart(t) && IsResource(t))
                .ToList();
            if (brokenTypes.Count > 0)
            {
                throw new LibrettoException($"Classes cannot be Resources and ResourceParts in the same time: {brokenTypes.Select(t => t.Name).ToArray()}");
            }
        }

        private static bool IsPart(Type t)
        {
            return typeof(IResourcePart).IsAssignableFrom(t);
        }

        private static bool IsResource(Type t)
        {
            return t.IsSubclassOf(typeof(Resource));
        }
    }
}