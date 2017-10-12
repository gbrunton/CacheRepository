using System;
using System.Linq;

namespace CacheRepository.Utils
{
    public static class TypeExtensions
    {
        public static bool IsSimple(this Type source)
        {
            return
                source.IsValueType ||
                source.IsPrimitive ||
                new[] {
                    typeof(String),
                    typeof(Decimal),
                    typeof(DateTime),
                    typeof(DateTimeOffset),
                    typeof(TimeSpan),
                    typeof(Guid)
                }.Contains(source) ||
                Convert.GetTypeCode(source) != TypeCode.Object;
        }
    }
}