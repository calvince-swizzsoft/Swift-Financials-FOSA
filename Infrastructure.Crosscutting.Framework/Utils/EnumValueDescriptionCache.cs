using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Infrastructure.Crosscutting.Framework.Utils
{
    /// <summary>
    /// Caches the "enum objects" for the lifetime of the application.
    /// </summary>
    public static class EnumValueDescriptionCache
    {
        private static readonly IDictionary<Type, Tuple<object[], string[]>> _cache = new Dictionary<Type, Tuple<object[], string[]>>();

        public static Tuple<object[], string[]> GetValues(Type type)
        {
            if (!type.IsEnum)
                throw new ArgumentException("Type '" + type.Name + "' is not an enum");

            Tuple<object[], string[]> values;

            if (!_cache.TryGetValue(type, out values))
            {
                FieldInfo[] fieldInfos = type.GetFields()
                    .Where(f => f.IsLiteral)
                    .ToArray();

                object[] enumValues = fieldInfos.Select(f => f.GetValue(null)).ToArray();

                DescriptionAttribute[] descriptionAttributes = fieldInfos
                    .Select(f => f.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault())
                    .OfType<DescriptionAttribute>()
                    .ToArray();

                string[] descriptions = descriptionAttributes.Select(a => a.Description).ToArray();

                Debug.Assert(enumValues.Length == descriptions.Length, "Each Enum value must have a description attribute set");

                List<KeyValuePair<string, string>> kvpList = new List<KeyValuePair<string, string>>();

                for (int i = 0; i < enumValues.Length; i++)
                {
                    KeyValuePair<string, string> kvp = new KeyValuePair<string, string>(string.Format("{0}", enumValues[i]), descriptions[i]);

                    kvpList.Add(kvp);
                }

                kvpList.Sort((a, b) => a.Key.CompareTo(b.Key));

                _cache[type] = values = new Tuple<object[], string[]>(kvpList.Select(x => (object)x.Key).ToArray(), kvpList.Select(x => x.Value).ToArray());
            }

            return values;
        }
    }
}
