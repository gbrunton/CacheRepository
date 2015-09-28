using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace CacheRepository.EntityRetrieverStrategies.GetAllQueryStrategies
{
    // http://codereview.stackexchange.com/questions/1002/mapping-expandoobject-to-another-object-type
    public static class Mapper<T>
        // We can only use reference types
        where T : class
    {
        private static readonly Dictionary<string, PropertyInfo> _propertyMap;

        static Mapper()
        {
            // At this point we can convert each
            // property name to lower case so we avoid 
            // creating a new string more than once.
            _propertyMap =
                typeof(T)
                .GetProperties()
                .ToDictionary(
                    p => p.Name.ToLower(),
                    p => p
                );
        }

        public static void Map(ExpandoObject source, T destination)
        {
            // Might as well take care of null references early.
            if (source == null)
                throw new ArgumentNullException("source");
            if (destination == null)
                throw new ArgumentNullException("destination");

            // By iterating the KeyValuePair<string, object> of
            // source we can avoid manually searching the keys of
            // source as we see in your original code.
            foreach (var kv in source)
            {
                PropertyInfo p;
                if (_propertyMap.TryGetValue(kv.Key.ToLower(), out p))
                {
                    p.SetValue(destination, kv.Value, null);
                }
            }
        }
    }
}