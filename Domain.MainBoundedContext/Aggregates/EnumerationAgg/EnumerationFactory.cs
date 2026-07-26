using System;

namespace Domain.MainBoundedContext.Aggregates.EnumerationAgg
{
    public static class EnumerationFactory
    {
        public static Enumeration CreateEnumeration(string key, int value, string description)
        {
            var enumeration = new Enumeration();

            enumeration.GenerateNewIdentity();

            enumeration.Key = key;

            enumeration.Value = value;

            enumeration.Description = description;

            enumeration.CreatedDate = DateTime.Now;

            return enumeration;
        }
    }
}
