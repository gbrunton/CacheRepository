using System;
using System.Reflection;
using CacheRepository.Utils;

namespace CacheRepository.OutputConventions
{
	public class DateTimeToyyyyMMdd : IOutputConvention
	{
		private readonly Cache<Type, bool> shouldBeAppliedCache;

		public DateTimeToyyyyMMdd()
		{
			this.shouldBeAppliedCache = new Cache<Type, bool>
			{
				OnMissing =
					propertyType =>
					propertyType == typeof(DateTime) || propertyType == typeof(DateTime?)
			};
		}

		public bool ShouldBeApplied(Type entityType, PropertyInfo property, object valueAsObj)
		{
			return this.shouldBeAppliedCache[property.PropertyType];
		}

		public object Apply(object valueAsObj)
		{
			return valueAsObj == null 
				? string.Empty 
				: ((DateTime) valueAsObj).ToString("yyyyMMdd");
		}
	}
}