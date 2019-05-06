using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using libra.core.exceptions;
using libra.core.utils;

namespace libra.core
{
    public class ResourceSystem : IResourceSystem
    {
        private readonly Dictionary<Guid, Resource> _resById = new Dictionary<Guid, Resource>();
        private readonly Dictionary<Type, IList> _resByType = new Dictionary<Type, IList>();
        private readonly IResourceLogger _logger;

        private ResourceSystem(IResourceLogger logger)
        {
            _logger = logger;
        }

        public T Resolve<T>(ResourceRef<T> reference) where T : Resource
        {
            if (reference == null)
            {
                throw new NullResourceReferenceException();
            }

            return _resById.GetValueOrDefault(reference.ResourceId) as T;
        }

        public IEnumerable<T> GetAll<T>() where T : Resource
        {
            IList result;
            if (_resByType.TryGetValue(typeof(T), out result))
            {
                return result.Cast<T>();
            }

            _logger.LogWarn("Cannot find ");
            return new List<T>();
        }

        public static IResourceSystem Create(ResourceConfigs config, IEnumerable<Resource> resources)
        {
            var logger = config.Logger ?? DummyLogger.Instance;
            var resourceSystem = new ResourceSystem(logger);

            foreach (var resource in resources)
            {
                if (resourceSystem._resById.TryAdd(resource.ResourceId, resource))
                {
                    var list = resourceSystem._resByType.GetValueOrDefault(resource.GetType());
                    if (list == null)
                    {
                        list = new List<Resource>();
                        resourceSystem._resByType.Add(resource.GetType(), list);
                    }

                    list.Add(resource);
                }
                else
                {
                    logger.LogWarn(string.Format("Duplicate id: {0}, ignoring", resource.ResourceId));
                }
            }

            logger.LogInfo(string.Format("Resource system initialized, total: {0}", resourceSystem._resById.Count));
            return resourceSystem;
        }
    }
}