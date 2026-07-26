using Application.MainBoundedContext.DTO;
using Domain.MainBoundedContext.Aggregates.EnumerationAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Application.MainBoundedContext.Services
{
    public class EnumerationAppService : IEnumerationAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<Enumeration> _enumerationRepository;

        public EnumerationAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<Enumeration> enumerationRepository)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (enumerationRepository == null)
                throw new ArgumentNullException(nameof(enumerationRepository));

            _dbContextScopeFactory = dbContextScopeFactory;
            _enumerationRepository = enumerationRepository;
        }

        public bool SeedEnumerations(ServiceHeader serviceHeader)
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var existing = FindEnumerations(string.Empty, serviceHeader);

                if (existing != null && existing.Any())
                {
                    foreach (var item in existing)
                    {
                        var enumeration = _enumerationRepository.Get(item.Id, serviceHeader);

                        if (enumeration != null)
                        {
                            _enumerationRepository.Remove(enumeration, serviceHeader);
                        }
                    }
                }

                var assemblies = AppDomain.CurrentDomain.GetAssemblies();

                foreach (var assembly in assemblies)
                {
                    if (assembly.FullName.StartsWith("Infrastructure.Crosscutting.Framework", StringComparison.OrdinalIgnoreCase))
                    {
                        var enumTypes = assembly.GetTypes().Where(t => t.IsEnum);

                        foreach (var type in enumTypes)
                        {
                            var name = type.Name;

                            var tuple = EnumValueDescriptionCache.GetValues(type);

                            for (int n = 0; n < tuple.Item1.Length; n++)
                            {
                                var enumeration = EnumerationFactory.CreateEnumeration(type.Name, tuple.Item1[n], tuple.Item2[n]);

                                _enumerationRepository.Add(enumeration, serviceHeader);
                            }
                        }
                    }
                }

                return dbContextScope.SaveChanges(serviceHeader) >= 0;
            }
        }

        public EnumerationDTO FindEnumeration(Guid enumerationId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return _enumerationRepository.Get<EnumerationDTO>(enumerationId, serviceHeader);
            }
        }

        public List<EnumerationDTO> FindEnumerations(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return _enumerationRepository.GetAll<EnumerationDTO>(serviceHeader);
            }
        }

        public PageCollectionInfo<EnumerationDTO> FindEnumerations(int pageIndex, int pageSize, string text, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = EnumerationSpecifications.EnumerationFullText(text);

                ISpecification<Enumeration> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                return _enumerationRepository.AllMatchingPaged<EnumerationDTO>(spec, pageIndex, pageSize, sortFields, true, serviceHeader);
            }
        }

        public List<EnumerationDTO> FindEnumerations(string text, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = EnumerationSpecifications.EnumerationFullText(text);

                ISpecification<Enumeration> spec = filter;

                return _enumerationRepository.AllMatching<EnumerationDTO>(spec, serviceHeader);
            }
        }
    }

    /// <summary>
    /// Caches the "enum objects" for the lifetime of the application.
    /// </summary>
    static class EnumValueDescriptionCache
    {
        private static readonly IDictionary<Type, Tuple<int[], string[]>> _cache = new Dictionary<Type, Tuple<int[], string[]>>();

        public static Tuple<int[], string[]> GetValues(Type type)
        {
            if (!type.IsEnum)
                throw new ArgumentException("Type '" + type.Name + "' is not an enum");

            Tuple<int[], string[]> values;

            if (!_cache.TryGetValue(type, out values))
            {
                FieldInfo[] fieldInfos = type.GetFields()
                    .Where(f => f.IsLiteral)
                    .ToArray();

                int[] enumValues = fieldInfos.Select(f => (int)f.GetValue(null)).ToArray();

                DescriptionAttribute[] descriptionAttributes = fieldInfos
                    .Select(f => f.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault())
                    .OfType<DescriptionAttribute>()
                    .ToArray();

                string[] descriptions = descriptionAttributes.Select(a => a.Description).ToArray();

                Debug.Assert(enumValues.Length == descriptions.Length, "Each Enum value must have a description attribute set");

                _cache[type] = values = new Tuple<int[], string[]>(enumValues, descriptions);
            }

            return values;
        }
    }
}
