using System;
using System.Reflection;

namespace CacheRepository.OutputConventions
{
	public class PropertyName_Length : IOutputConvention
	{
		public bool ShouldBeApplied(Type entityType, PropertyInfo property, object valueAsObj)
		{
			throw new NotImplementedException();
		}

		public object Apply(object valueAsObj)
		{
			throw new NotImplementedException();
		}
	}
}