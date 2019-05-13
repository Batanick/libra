using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using libra.core;
using libretto.exceptions;
using libretto.model;
using libretto.utils;
using NLog;
using PropertyInfo = libretto.model.PropertyInfo;

namespace libretto
{
    public class ClassAnalyser
    {
        private static readonly Logger Log = LogManager.GetLogger(nameof(ClassAnalyser));

        private static readonly Dictionary<Type, ObjectType> PrimitiveTypesMapping = new Dictionary<Type, ObjectType>
        {
            {typeof(long), ObjectType.integer},
            {typeof(int), ObjectType.integer},
            {typeof(byte), ObjectType.integer},

            {typeof(float), ObjectType.number},
            {typeof(double), ObjectType.number},

            {typeof(string), ObjectType.@string},
            {typeof(bool), ObjectType.boolean}
        };

        private readonly List<Type> _resources = new List<Type>();
        private readonly List<Type> _parts = new List<Type>();

        public List<ResourceInfo> Process(List<Assembly> assemblies)
        {
            var types = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => !t.IsAbstract)
                .Where(t => !t.IsInterface)
                .ToList();

            CheckTypes(types);
            _resources.AddRange(types.Where(IsResource));
            _parts.AddRange(types.Where(IsPart));

            var result = new List<ResourceInfo>();
            foreach (var res in _resources)
            {
                result.Add(ProcessType(res, true));
            }

            foreach (var part in _parts)
            {
                result.Add(ProcessType(part, false));
            }

            return result;
        }

        private ResourceInfo ProcessType(Type t, bool resource)
        {
            Log.Info($"Processing: {t.Name}");
            var name = ReflectionHelper.GetName(t);

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

                var processed = ProcessProperty(info);
                if (processed != null)
                {
                    props.Add(processed);
                }
            }

            return new ResourceInfo
            {
                Id = name,
                Properties = props,
                Type = resource ? ResourceType.resource : ResourceType.part
            };
        }

        private PropertyInfo ProcessProperty(System.Reflection.PropertyInfo info)
        {
            var title = ReflectionHelper.GetPropertyTitle(info);

            if (info.PropertyType.IsArray || typeof(ICollection).IsAssignableFrom(info.PropertyType))
            {
                var elementType = GetElementType(info.PropertyType);
                var elementInfo = ProcessInfoProperty(info.Name, title, elementType);
                
                // do not care here, since all info would be in the parent PropertyInfo
                elementInfo.Name = null;
                elementInfo.Title = null;

                return new PropertyInfo
                {
                    Name = info.Name,
                    Title = title,
                    Elements = elementInfo,
                    Type = ObjectType.array
                };
            }

            return ProcessInfoProperty(info.Name, title, info.PropertyType);
        }

        private PropertyInfo ProcessInfoProperty(string name, string title, Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ResourceRef<>))
            {
                var resType = type.GetGenericArguments()[0];
                var compatibleTypes = GetCompatibleTypes(_resources, resType);
                if (compatibleTypes.Count == 0)
                {
                    Log.Warn($"Unable to find potential reference candidates for: {resType.Name}, ignoring {name}");
                    return null;
                }

                return new PropertyInfo
                {
                    Name = name,
                    Type = ObjectType.@ref,
                    Title = title,
                    AllowedTypes = compatibleTypes
                };
            }

            if (IsPart(type))
            {
                var compatibleTypes = GetCompatibleTypes(_parts, type);
                if (compatibleTypes.Count == 0)
                {
                    Log.Warn($"Unable to find potential candidates for: {type.Name}, ignoring {name}");
                    return null;
                }

                return new PropertyInfo
                {
                    Name = name,
                    Type = ObjectType.@object,
                    Title = title,
                    AllowedTypes = compatibleTypes
                };
            }

            if (PrimitiveTypesMapping.TryGetValue(type, out var objectType))
            {
                return new PropertyInfo
                {
                    Name = name,
                    Title = title,
                    Type = objectType
                };
            }

            throw new LibrettoException($"Unable process property {name}, unknown type: {type}");
        }

        private List<string> GetCompatibleTypes(List<Type> types, Type expected)
        {
            return types
                .Where(expected.IsAssignableFrom)
                .Select(ReflectionHelper.GetName)
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

        public static Type GetElementType(Type type)
        {
            return type.IsArray ? type.GetElementType() : type.GetGenericArguments()[0];
        }
    }
}