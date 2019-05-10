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

        public List<ResourceType> Process(List<Assembly> assemblies)
        {
            var types = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => !t.IsAbstract)
                .ToList();

            CheckTypes(types);

            var result = new List<ResourceType>();
            var resTypes = types.Where(IsResource).ToList();
            foreach (var res in resTypes)
            {
                result.Add(ProcessType(res));
            }

            var partTypes = types.Where(IsPart).ToList();
            foreach (var res in partTypes)
            {
                result.Add(ProcessType(res));
            }

            return result;
        }

        private static ResourceType ProcessType(Type t)
        {
            _log.Info($"Processing: {t.Name}");
            var name = ReflectionHelper.GetResourceName(t);

            var props = new List<PropertyInfo>();
            foreach (var info in t.GetProperties())
            {
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

        private static PropertyInfo ProcessInfoProperty(System.Reflection.PropertyInfo info)
        {
            var name = ReflectionHelper.GetPropertyName(info);
            if (!PrimitiveTypesMapping.TryGetValue(info.PropertyType, out var type))
            {
                _log.Warn($"Unable to process property of type ${info.PropertyType}, ignoring");
                return null;
            }

            return new PropertyInfo
            {
                Name = name,
                Title = name,
                Type = type
            };
        }

        private static void CheckTypes(List<Type> types)
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